
using Azure;
using MainApp.Data;
using MainApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MainApp.Infrastructure.Authentication
{
    // This thing simply tells the page whether the user is authentified or not - so that we don't mindlessly authentify
    // everyone who visits the front page with a cookie
    public class BimodalCheckAuthentication : IAsyncPageFilter
    {

        protected readonly UserManager<IdentityUser> _userManager;
        protected readonly ApplicationDbContext _db;

        public BimodalCheckAuthentication(UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            this._userManager = userManager;
            this._db = db;
        }

        protected async Task<AnonUser?> CheckAnonUserAsync(HttpRequest request)
        {
            if (request.Cookies.TryGetValue(Consts.ANON_ID_COOKIE_NAME, out var cookieString))
            {
                try
                {
                    // we extracted the string, yet it might not be valid JSON
                    var userData = JsonSerializer.Deserialize<AnonUserCookieModel>(cookieString) ?? throw new JsonException();
                    return  await this._db.AnonUsers
                        .SingleAsync(t => t.AnonUserId == userData.AnonUserId && t.PublicToken == userData.PublicToken);
                }
                catch (JsonException)
                {
                    System.Diagnostics.Debug.WriteLine("Anonymous state cookie was found, but JSON is invalid.");
                }
                // if the user from the cookie somehow doesn't exist or is duplicated - just reset
                catch (InvalidOperationException)
                {
                    System.Diagnostics.Debug.WriteLine("Anonymous state cookie was found, but values do not matchup with DB.");
                }
            }

            // no valid anon
            return null;
        }

        protected void UpdateAnonCookie(HttpResponse response, AnonUser anonUser)
        {
            // we update the cookie expiration
            response.Cookies.Append(
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
        }

        protected async Task<IdentityUser?> CheckIdentityUserAsync(HttpRequest request)
        {
            return await this._userManager.GetUserAsync(request.HttpContext.User);
        }

        protected async Task<UserType> CheckAndWriteUserDetailsAsync(PageHandlerExecutingContext context)
        {
            HttpRequest request = context.HttpContext.Request;
            var identityUser = await this.CheckIdentityUserAsync(request);
            var anonUser = await this.CheckAnonUserAsync(request);
            UserType userType;

            // we need to communicate our findings to the page, so we save them to the HttpContext
            if (identityUser != null)
            {
                userType = UserType.IdentityUser;
                context.HttpContext.Items[Consts.AUTH_CONTEXT_IDENTITY_USER_KEY] = identityUser;
            }
            else if (anonUser != null)
            {
                userType = UserType.AnonUser;
                context.HttpContext.Items[Consts.AUTH_CONTEXT_ANON_USER_KEY] = anonUser;

                this.UpdateAnonCookie(context.HttpContext.Response, anonUser);
            }
            else
            {
                userType = UserType.None;
            }
            context.HttpContext.Items[Consts.AUTH_CONTEXT_USER_TYPE_KEY] = userType;
            return userType;
        }

        public virtual async Task OnPageHandlerExecutionAsync(
            PageHandlerExecutingContext context,
            PageHandlerExecutionDelegate next)
        {
            await this.CheckAndWriteUserDetailsAsync(context);

            await next();
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context) => Task.CompletedTask;
    }
}
