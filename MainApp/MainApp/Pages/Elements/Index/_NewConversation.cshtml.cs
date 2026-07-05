using MainApp.Data;
using MainApp.Infrastructure.Authentication;
using MainApp.Infrastructure.Conversations;
using MainApp.Infrastructure.Page;
using MainApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection.PortableExecutable;

namespace MainApp.Pages.Elements.Index
{
    [BimodalAuthentify]
    public class NewConversationPageModel : BimodalAuthConvPageModel
    {
        public NewConversationPageModel(ApplicationDbContext db, IConversationsService conversationsService) : base(db, conversationsService)
        {
        }

        public async Task<IActionResult> OnPostAsync()
        { 
            return Page();
        }
    }
}
