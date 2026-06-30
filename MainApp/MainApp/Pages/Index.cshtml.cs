using MainApp.Data;
using MainApp.Models;
using MainApp.Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MainApp.Infrastructure.Page;
using System.Text.Json;
using MainApp.Infrastructure.Conversations;

namespace MainApp.Pages
{
    [BimodalCheckAuthentication]
    public class IndexPageModel : PageModel
    {
        // private readonly Conversation dummyConvo 
        public IndexPageModel()
        {
            
        }
        
        // all the data in the HttpContext comes from the bimodal authentication attribute
        // at this point I will require authentication because conversation must be created
        public async Task OnGetAsync()
        {
            
        }
    }
}
