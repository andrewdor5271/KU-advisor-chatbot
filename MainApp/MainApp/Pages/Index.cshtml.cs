using MainApp.Data;
using MainApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices.Swift;

namespace MainApp.Pages
{
    public class IndexPageModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager = null!;
        public int ConversationId { get; private set; }
        public bool Authorized { get; private set; }
        public List<Conversation> Conversations { get; private set; } = null!;
        public IndexPageModel(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            this._db = db;
            this._userManager = userManager;
        }
        public async Task OnGetAsync()
        {
            //this.ConversationId = ConversationID;
            this.ConversationId = 1;

            IdentityUser? user = await this._userManager.GetUserAsync(HttpContext.User);

            // temporary before cookies
            // not [Authorize] yet because of local store requirements
            if (user == null)
            {
                this.Authorized = false;
                this.Conversations = new List<Conversation>
                {
                    new Conversation
                    {
                        Title = "No send send yet COOKED",
                        CreationDatetime = DateTime.Now
                    }
                };
                return;
            }
            this.Authorized = true;
            this.Conversations = await _db.Conversations
                .Where(t => t.UserId == user.Id)
                .OrderByDescending(t => t.ConversationId) // newest convo on top
                .ToListAsync();
        }
    }
}
