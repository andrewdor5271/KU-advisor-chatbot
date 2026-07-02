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
        public IdentityUser? IdentityUser { get; private set; } = null;
        public IndexPageModel()
        {
            
        }
        
        public async Task OnGetAsync()
        {
            UserType userType = (UserType)HttpContext.Items[Consts.AUTH_CONTEXT_USER_TYPE_KEY]!;
            if(userType == UserType.IdentityUser)
            {
                this.IdentityUser = (IdentityUser)HttpContext.Items[Consts.AUTH_CONTEXT_IDENTITY_USER_KEY]!;
            }
        }
    }
}
