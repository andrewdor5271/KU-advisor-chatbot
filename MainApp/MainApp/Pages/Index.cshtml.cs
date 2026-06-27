using MainApp.Data;
using MainApp.Models;
using MainApp.Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MainApp.Pages
{
    [BimodalAuthentify]
    public class IndexPageModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager = null!;
        public bool Authorized { get; private set; }
        public List<Conversation> Conversations { get; private set; } = null!;
        public IndexPageModel(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            this._db = db;
            this._userManager = userManager;
        }
        
        // all the data in the HttpContext comes from the bimodal authentication attribute
        // at this point I will require authentication because conversation must be created
        public async Task OnGetAsync()
        {
            var userType = (UserType)HttpContext.Items[Consts.AUTH_CONTEXT_USER_TYPE_KEY]!;

            if (userType == UserType.AnonUser)
            {
                this.Authorized = false;
                var anonUser = (AnonUser)HttpContext.Items[Consts.AUTH_CONTEXT_ANON_USER_KEY]!;

                this.Conversations = await _db.Conversations
                .Where(t => t.AnonUserId == anonUser.AnonUserId)
                .OrderByDescending(t => t.ConversationId)
                .ToListAsync();

            }
            else
            {
                this.Authorized = true;
                var identityUser = (IdentityUser)HttpContext.Items[Consts.AUTH_CONTEXT_IDENTITY_USER_KEY]!;
                this.Conversations = await _db.Conversations
                    .Where(t => t.IdentityUserId == identityUser.Id)
                    .OrderByDescending(t => t.ConversationId) // newest convo on top
                    .ToListAsync();
            }
        }
    }
}
