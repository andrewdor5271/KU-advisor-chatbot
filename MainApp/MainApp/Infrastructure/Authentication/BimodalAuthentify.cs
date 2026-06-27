using Azure;
using Azure.Core;
using MainApp.Data;
using MainApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Infrastructure.Authentication
{
    // and this one actually creates a new cookie if needed unlike its parent -
    // so this is to be used on authentication-mandatory pages only
    public class BimodalAuthentify : BimodalCheckAuthentication
    {
        public BimodalAuthentify(UserManager<IdentityUser> userManager, ApplicationDbContext db) : base(userManager, db)
        { }

        protected async Task CreateAnonAsync(HttpContext context)
        {
            AnonUser newAnonUser = new AnonUser();
            DateTime creationDateTime = DateTime.UtcNow;
            Conversation firstConversation = new Conversation
            {
                Title = "Your conversation",
                CreationDatetime = creationDateTime,
                LastChangeDatetime = creationDateTime
            };
            firstConversation.SetUser(null, newAnonUser);
            this._db.AnonUsers.Add(newAnonUser);
            this._db.Conversations.Add(firstConversation);

            await this._db.SaveChangesAsync();

            this.UpdateAnonCookie(context.Response, newAnonUser);

            // and rewrite
            context.Items[Consts.AUTH_CONTEXT_USER_TYPE_KEY] = UserType.AnonUser;
            context.Items[Consts.AUTH_CONTEXT_ANON_USER_KEY] = newAnonUser;
        }

        // this runs before the page loads. Thus, it creates the user only when we need to init the conversation
        // if we put it on the right page (actual message being sent)
        // not implemented yet btw cause it's hard to not request anything in the beginning
        public override async Task OnPageHandlerExecutionAsync(
            PageHandlerExecutingContext context,
            PageHandlerExecutionDelegate next)
        {
            var userType = await this.CheckAndWriteUserDetailsAsync(context);

            if (userType == UserType.None)
            {
                await this.CreateAnonAsync(context.HttpContext);
            }

            await next();
        }
    }
}
