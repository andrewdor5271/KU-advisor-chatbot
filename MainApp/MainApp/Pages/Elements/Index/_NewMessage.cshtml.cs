using Grpc.Core;
using MainApp.Data;
using MainApp.Grpc.Protos;
using MainApp.Infrastructure.Authentication;
using MainApp.Infrastructure.Conversations;
using MainApp.Infrastructure.Page;
using MainApp.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MainApp.Pages.Elements.Index
{
    [BimodalAuthentify]
    public class NewMessagePageModel : BimodalAuthMessagePageModel
    {
        private readonly MessageService.MessageServiceClient _grpcService;
        private readonly IConversationsService _conversationsService;
        public Message ReceivedMessage { get; private set; } = null!;
        public Message ResponseMessage { get; private set; } = null!;
        public bool DummyConvoFlag { get; private set; }
        public NewMessagePageModel(ApplicationDbContext db,
            IConversationsService conversationsService,
            MessageService.MessageServiceClient grpcService) : base(db)
        {
            this._conversationsService = conversationsService;
            this._grpcService = grpcService;
        }
        private async Task<Message> SaveBotMessageAsync(int conversationId, String text)
        {
            var message = new Message
            {
                SenderType = SenderType.Bot,
                ConversationId = conversationId,
                Text = text,
                CreationDatetime = DateTime.UtcNow
            };
            this._db.Messages.Add(message);
            await this._db.SaveChangesAsync();
            return message;
        }
        public async Task<IActionResult> OnPostAsync(int? conversationId, String text)
        {
            UserType userType = (UserType)HttpContext.Items[Consts.AUTH_CONTEXT_USER_TYPE_KEY]!;
            if(conversationId == null)
            {
                this.DummyConvoFlag = true;
                // We need to seriously rearrange the whole page if we create a new convo
                // Thus, we send back an event - so a lot of HTMX is triggered
                Response.Headers.Append("HX-Trigger", "conversation-change");
                if (userType == UserType.AnonUser)
                {
                    var anonUser = (AnonUser)HttpContext.Items[Consts.AUTH_CONTEXT_ANON_USER_KEY]!;
                    var convo = 
                        await this._conversationsService
                        .CreateForAnonAsync(anonUser.AnonUserId, Consts.DUMMY_CONVO_DEFAULT_TITLE);
                    conversationId = convo.ConversationId;
                }
                else if(userType == UserType.IdentityUser)
                {
                    var identityUser = (IdentityUser)HttpContext.Items[Consts.AUTH_CONTEXT_IDENTITY_USER_KEY]!;
                    var convo = 
                        await this._conversationsService
                        .CreateForIdentityAsync(identityUser.Id, Consts.DUMMY_CONVO_DEFAULT_TITLE);
                    conversationId = convo.ConversationId;
                }
                else
                {
                    return Forbid();
                }
            }
            else if(!await CheckMessagesSecurityAsync((int)conversationId))
            {
                return Forbid();
            }
            else
            {
                this.DummyConvoFlag = false;
            }

            // and here we process everything that's actually not about creating new conversations
            this.ReceivedMessage = new Message
            {
                SenderType = SenderType.User,
                ConversationId = (int)conversationId,
                Text = text,
                CreationDatetime = DateTime.UtcNow
            };
            // Using the default message class for the model in response partial is a bad decision
            // But not too bad so I don't really care
            this.ResponseMessage = new Message
            {
                ConversationId = (int)conversationId,
                SenderType = SenderType.Bot,
                CreationDatetime = DateTime.UtcNow,
            };

            this._db.Messages.Add(this.ReceivedMessage);
            await this._db.SaveChangesAsync();

            return Page();
        }
        public async Task<IActionResult> OnGetResponseAsync(int conversationId)
        {
            if (!await this.CheckMessagesSecurityAsync(conversationId))
            {
                return Forbid();
            }

            // Using the default message class for the model in response partial is a bad decision
            // But not too bad so I don't really care
            this.ResponseMessage = new Message
            {
                ConversationId = conversationId,
                SenderType = SenderType.Bot,
                CreationDatetime = DateTime.UtcNow,
            };

            var call = this._grpcService.GenerateReply(
                new NewMessageRequest
                {
                    ConversationId = (int)conversationId
                },
                cancellationToken: HttpContext.RequestAborted
            );

            String cumulativeText = "";
            String textBuffer = "";
            var responseBodyFeature = HttpContext.Features.Get<IHttpResponseBodyFeature>(); // to disable buffering on small messages
            if (responseBodyFeature != null)
            {
                responseBodyFeature.DisableBuffering();
            }
            Response.ContentType = "text/event-stream"; // for sse
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Connection", "keep-alive");

            try
            {
                await foreach (var responseChunk in call.ResponseStream.ReadAllAsync())
                {
                    if (responseChunk.ConversationId != conversationId)
                    {
                        await Response.WriteAsync("event: error\ndata: {}\n\n");
                        await Response.Body.FlushAsync();
                        break;
                    }
                    switch (responseChunk.EventCase)
                    {
                        case NewMessageChunkResponse.EventOneofCase.Token:
                            textBuffer += responseChunk.Token.Text;
                            break;
                        case NewMessageChunkResponse.EventOneofCase.Completion:
                            cumulativeText += textBuffer;
                            await Response.WriteAsync($"event: chunk\ndata: {cumulativeText}\n\nevent: done\ndata: {{}}\n\n");
                            await Response.Body.FlushAsync();
                            await this.SaveBotMessageAsync(conversationId, responseChunk.Completion.FullText);
                            return new EmptyResult();
                        case NewMessageChunkResponse.EventOneofCase.Error:
                            break;
                    }

                    if (textBuffer.Length >= Consts.TEXT_CHUNK_FLUSH_LENGTH)
                    {
                        cumulativeText += textBuffer;
                        await Response.WriteAsync($"event: chunk\ndata: {cumulativeText}\n\n");
                        await Response.Body.FlushAsync();
                        textBuffer = "";
                    }
                }
            }
            catch(Exception e)
            {
                // CHANGE BEFORE PROD
                await Response.WriteAsync($"event: error\ndata: {e.Message}\n\nevent: done\ndata:");
                await Response.Body.FlushAsync();
                throw;
            }
            return new EmptyResult();
        }
    }
}
