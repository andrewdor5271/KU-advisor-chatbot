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
    [BimodalCheckAuthentication]
    public class ConversationsPageModel : BimodalAuthConvPageModel
    {
        public ConversationsPageModel(ApplicationDbContext db, IConversationsService conversationsService) :
            base(db, conversationsService)
        {
        }
        public async Task OnGetAsync()
        {
            await this.InitializeConversationsAsync(false); // overriding user check so it returns empty list when we don't have a user
        }

        public async Task<IActionResult> OnPostRenameAsync(int conversationId, String title)
        {
            Conversation convo;
            try
            {
                convo = await this.LoadConversationAsync(conversationId);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }

            convo.Title = title;
            await this._db.SaveChangesAsync();

            Response.Headers.Append("HX-Trigger", "conversation-change");
            return new NoContentResult();
        }
    }
}
