using MainApp.Data;
using MainApp.Infrastructure.Authentication;
using MainApp.Infrastructure.Conversations;
using MainApp.Infrastructure.Page;
using MainApp.Models;
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
            await this.LoadConversations(false); // overriding user check so it returns empty list when we don't have a user
        }
    }
}
