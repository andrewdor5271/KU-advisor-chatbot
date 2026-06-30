using MainApp.Data;
using MainApp.Infrastructure.Authentication;
using MainApp.Infrastructure.Conversations;
using MainApp.Infrastructure.Page;
using MainApp.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MainApp.Pages.Elements.Index
{
    [BimodalAuthentify]
    public class NewMessagePageModel : BimodalAuthMessagePageModel
    {
        private readonly IConversationsService _conversationsService;
        public Message Message { get; private set; } = null!;
        public NewMessagePageModel(ApplicationDbContext db, IConversationsService conversationsService) : base(db)
        {
            this._conversationsService = conversationsService;
        }
        public async Task<IActionResult> OnPostAsync(int? conversationId, String text)
        {
            UserType userType = (UserType)HttpContext.Items[Consts.AUTH_CONTEXT_USER_TYPE_KEY]!;
            if(conversationId == null)
            {
                // We need to seriously rearrange the whole page if we create a new convo
                // Thus, we send back an event - so a lot of HTMX is triggered
                Response.Headers.Append("HX-Trigger", "conversation-created");
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

            // and here we process everything that's actually not about creating new conversations
            this.Message = new Message
            {
                ConversationId = (int)conversationId,
                Text = text,
                CreationDatetime = DateTime.Now
            };

            this._db.Messages.Add(this.Message);
            await this._db.SaveChangesAsync();
            return Page();
        }
}
}
