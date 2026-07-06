using MainApp.Data;
using MainApp.Infrastructure.Authentication;
using MainApp.Infrastructure.Conversations;
using MainApp.Infrastructure.Page;
using MainApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Pages.Elements.Index
{
    [BimodalCheckAuthentication]
    public class FAQDialogBodyPageModel : BimodalAuthConvPageModel
    {
        public FAQDialogBodyPageModel(ApplicationDbContext db, IConversationsService conversationsService) : base(db, conversationsService)
        {
        }

        public bool IsValidIdentityUser { get; private set; } = false;
        public int ConversationId { get; private set; }
        public async Task<IActionResult> OnGetAsync(int conversationId)
        {
            this.ConversationId = conversationId;
            UserType userType = (UserType)HttpContext.Items[Consts.AUTH_CONTEXT_USER_TYPE_KEY]!;
            if(userType != UserType.IdentityUser)
            {
                return Page();
                
            }

            this.IsValidIdentityUser = true;
            Conversation convo;
            try
            {
                convo = await this.LoadConversationAsync(this.ConversationId);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int conversationId, string description)
        {
            this.ConversationId = conversationId;

            UserType userType = (UserType)HttpContext.Items[Consts.AUTH_CONTEXT_USER_TYPE_KEY]!;
            if (userType != UserType.IdentityUser)
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                return BadRequest("FAQ description is required.");
            }

            IdentityUser identityUser = (IdentityUser)HttpContext.Items[Consts.AUTH_CONTEXT_IDENTITY_USER_KEY]!;
            Conversation convo;
            try
            {
                convo = await this.LoadConversationAsync(this.ConversationId);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }

            var faqConversation = new FAQConversation
            {
                Description = description.Trim(),
                CreationDatetime = DateTime.UtcNow,
                IdentityUser = identityUser,
                IdentityUserId = identityUser.Id
            };
            _db.FAQConversations.Add(faqConversation);
            await _db.SaveChangesAsync();

            var messages = await _db.Messages
                .AsNoTracking()
                .Where(message => message.ConversationId == convo.ConversationId)
                .OrderBy(message => message.CreationDatetime)
                .ThenBy(message => message.MessageId)
                .ToListAsync();

            _db.FAQMessages.AddRange(messages.Select(message => new FAQMessage
            {
                Text = message.Text,
                CreationDatetime = message.CreationDatetime,
                SenderType = message.SenderType,
                FAQConversationId = faqConversation.FAQConversationId,
                FAQConversation = faqConversation
            }));
            await _db.SaveChangesAsync();

            Response.Headers["HX-Redirect"] = Url.Page("/FAQ");
            return new NoContentResult();
        }
    }
}
