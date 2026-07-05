using MainApp.Data;
using MainApp.Infrastructure.Authentication;
using MainApp.Infrastructure.Conversations;
using MainApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Infrastructure.Page
{
    // assumed that we will have the attributes marked
    public class BimodalAuthConvPageModel : PageModel
    {
        protected readonly ApplicationDbContext _db;
        protected readonly IConversationsService _conversationsService;
        public List<Conversation> Conversations { get; protected set; } = null!;
        public readonly Conversation DummyConversation;

        public BimodalAuthConvPageModel(ApplicationDbContext db, IConversationsService conversationsService) : base()
        {
            var type = this.GetType();

            // check if the attributes are set
            if (!type.IsDefined(typeof(BimodalCheckAuthenticationAttribute), inherit: true) &&
                !type.IsDefined(typeof(BimodalAuthentifyAttribute), inherit: true))
            {
                throw new InvalidOperationException("Bimodal authentication attributes are not set");
            }

            this._db = db;
            this._conversationsService = conversationsService;
            this.DummyConversation = new Conversation
            {
                Title = Consts.DUMMY_CONVO_DEFAULT_TITLE,
                CreationDatetime = DateTime.UtcNow,
                LastChangeDatetime = DateTime.UtcNow,
            };
        }

        protected async Task InitializeConversationsAsync(bool validUserCheck=true)
        {
            var userType = (UserType)this.HttpContext.Items[Consts.AUTH_CONTEXT_USER_TYPE_KEY]!;
            if(userType == UserType.None)
            {
                if (validUserCheck)
                {
                    throw new InvalidOperationException("Cannot load the conversation - neither authentication mode has a valid user");
                }
                this.Conversations = new List<Conversation>();
                return;
            }

            if (userType == UserType.AnonUser)
            {
                var anonUser = (AnonUser)this.HttpContext.Items[Consts.AUTH_CONTEXT_ANON_USER_KEY]!;

                this.Conversations = await this._conversationsService.GetForAnonUserAsync(anonUser.AnonUserId);

            }
            else
            {
                var identityUser = (IdentityUser)HttpContext.Items[Consts.AUTH_CONTEXT_IDENTITY_USER_KEY]!;
                this.Conversations = await this._conversationsService.GetForIdentityUserAsync(identityUser.Id);
            }
        }

        protected async Task<Conversation> LoadConversationAsync(int conversationId)
        {
            var userType = (UserType)this.HttpContext.Items[Consts.AUTH_CONTEXT_USER_TYPE_KEY]!;
            Conversation convo = await this._conversationsService.GetAsync(conversationId) ??
                throw new InvalidOperationException($"Cannot fetch the conversation if={conversationId}");

            if (userType == UserType.AnonUser)
            {
                var anonUser = (AnonUser)this.HttpContext.Items[Consts.AUTH_CONTEXT_ANON_USER_KEY]!;

                if (convo.AnonUserId == null || anonUser.AnonUserId != convo.AnonUserId)
                {
                    throw new UnauthorizedAccessException($"Anon user with id={anonUser.AnonUserId} doesn't match the convo with id={convo.ConversationId}");
                }
            }
            else if (userType == UserType.IdentityUser)
            {
                var identityUser = (IdentityUser)HttpContext.Items[Consts.AUTH_CONTEXT_IDENTITY_USER_KEY]!;

                if (convo.IdentityUserId == null || identityUser.Id != convo.IdentityUserId)
                {
                    throw new UnauthorizedAccessException($"Identity user with id={identityUser.Id} doesn't match the convo with id={convo.ConversationId}");
                }
            }

            return convo;
        }
    }
}
