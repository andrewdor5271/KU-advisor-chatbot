using MainApp.Data;
using MainApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MainApp.Pages
{
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
        
        // A method for parsing cookie-based anonymous user, or creating a state for a new one if not found

        private async Task<AnonUser> GetOrCreateAnonAsync()
        {
            if (Request.Cookies.TryGetValue(Consts.ANON_ID_COOKIE_NAME, out var cookieString))
            {
                try
                {
                    // we extracted the string, yet it might not be valid JSON
                    var userData = JsonSerializer.Deserialize<AnonUserCookieModel>(cookieString) ?? throw new JsonException();
                    return await _db.AnonUsers
                        .SingleAsync(t => t.AnonUserId == userData.AnonUserId && t.PublicToken == userData.PublicToken);
                }
                catch (JsonException)
                {
                    System.Diagnostics.Debug.WriteLine("Anonymous state cookie was found, but JSON is invalid. " +
                        "Resetting the anonymous user.");
                }
                // if the user from the cookie somehow doesn't exist or is duplicated - just reset
                catch(InvalidOperationException)
                {
                    System.Diagnostics.Debug.WriteLine("Anonymous state cookie was found, but values do not matchup with DB. " +
                        "Resetting the anonymous user.");
                }
                // reset happens because code continues to run after the catch blocks
            }

            // here we create a new one, because we somehow failed to find a good existing one (nonexistent or invalid)
            AnonUser newAnonUser = new AnonUser();
            DateTime creationDateTime = DateTime.Now;
            Conversation firstConversation = new Conversation
            {
                Title="Your conversation",
                CreationDatetime = creationDateTime,
                LastChangeDatetime = creationDateTime
            };
            firstConversation.SetUser(null, newAnonUser);
            _db.AnonUsers.Add(newAnonUser);
            _db.Conversations.Add(firstConversation);

            await _db.SaveChangesAsync();


            return newAnonUser;
        }
        public async Task OnGetAsync()
        {


            IdentityUser? identityUser = await this._userManager.GetUserAsync(HttpContext.User);

            // temporary before cookies
            // not [Authorize] yet because of local store requirements
            if (identityUser == null)
            {
                this.Authorized = false;
                AnonUser anonUser = await this.GetOrCreateAnonAsync();

                // create if not exists, update if exists (only the expiration changes)
                Response.Cookies.Append(
                    Consts.ANON_ID_COOKIE_NAME,
                    JsonSerializer.Serialize(new AnonUserCookieModel(anonUser)),
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddDays(Consts.ANON_ID_COOKIE_EXPIRATION_PERIOD)
                    }
                );

                this.Conversations = await _db.Conversations
                .Where(t => t.AnonUserId == anonUser.AnonUserId)
                .OrderByDescending(t => t.ConversationId)
                .ToListAsync();

                return;
            }

            this.Authorized = true;
            this.Conversations = await _db.Conversations
                .Where(t => t.IdentityUserId == identityUser.Id)
                .OrderByDescending(t => t.ConversationId) // newest convo on top
                .ToListAsync();
        }
    }
}
