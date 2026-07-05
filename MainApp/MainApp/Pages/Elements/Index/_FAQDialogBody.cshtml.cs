using MainApp.Data;
using MainApp.Infrastructure.Authentication;
using MainApp.Infrastructure.Conversations;
using MainApp.Infrastructure.Page;
using MainApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public async Task<IActionResult> OnPostAsync(int conversationId)
        {
            this.ConversationId = conversationId;
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

            Response.Headers["HX-Redirect"] = Url.Page("/FAQ");
            return new NoContentResult();
        }
}
}
