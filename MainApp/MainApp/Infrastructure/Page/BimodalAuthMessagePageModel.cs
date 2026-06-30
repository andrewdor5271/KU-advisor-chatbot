using MainApp.Data;
using MainApp.Infrastructure.Authentication;
using MainApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Infrastructure.Page
{
    public class BimodalAuthMessagePageModel: PageModel
    {
        protected readonly ApplicationDbContext _db;
        public BimodalAuthMessagePageModel(ApplicationDbContext db) : base()
        {
            var type = this.GetType();
            if (!type.IsDefined(typeof(BimodalCheckAuthenticationAttribute), inherit: true) &&
                !type.IsDefined(typeof(BimodalAuthentifyAttribute), inherit: true))
            {
                throw new InvalidOperationException("Bimodal authentication attributes are not set");
            }

            this._db = db;
        }
        protected async Task<bool> CheckMessagesSecurityAsync(int conversationId)
        {
            UserType userType = (UserType)this.HttpContext.Items[Consts.AUTH_CONTEXT_USER_TYPE_KEY]!;
            if (userType == UserType.None)
            {
                return false;
            }

            if (userType == UserType.AnonUser)
            {
                var anonUser = (AnonUser)this.HttpContext.Items[Consts.AUTH_CONTEXT_ANON_USER_KEY]!;
                var conversation = await this._db.Conversations.FindAsync(conversationId);
                if (conversation == null ||
                    conversation.AnonUserId == null ||
                    conversation.AnonUserId != anonUser.AnonUserId)
                {
                    return false;
                }
                
            }
            else if(userType == UserType.IdentityUser)
            {
                var identityUser = (IdentityUser)HttpContext.Items[Consts.AUTH_CONTEXT_IDENTITY_USER_KEY]!;
                var conversation = await this._db.Conversations.FindAsync(conversationId);
                if (conversation == null ||
                    conversation.IdentityUserId == null ||
                    conversation.IdentityUserId != identityUser.Id)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
