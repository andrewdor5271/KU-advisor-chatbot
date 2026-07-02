using MainApp.Data;
using MainApp.Infrastructure.Authentication;
using MainApp.Infrastructure.Page;
using MainApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Pages.Elements.Index
{
    [BimodalCheckAuthentication]
    public class MessagesPageModel : BimodalAuthMessagePageModel
    {
        public int? ConversationId { get; private set; }

        public UserType UserType { get; private set; }
        public MessagesPageModel(ApplicationDbContext db) : base(db)
        {
            
        }

        public async Task<IActionResult> OnGetAsync(int? conversationId, int? previousMessageId)
        {
            this.UserType = (UserType)HttpContext.Items[Consts.AUTH_CONTEXT_USER_TYPE_KEY]!;

            this.ConversationId = conversationId;
            if (this.ConversationId == null)
            {
                // we assume that it's a dummy page for new convos
                return Page();
            }

            
            if(!await CheckMessagesSecurityAsync((int)this.ConversationId))
            {
                return Forbid();
            }

            return Page();
        }
    }
}
