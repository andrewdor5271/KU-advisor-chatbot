using Microsoft.AspNetCore.Identity;

namespace MainApp.Models
{
    public class FAQConversation
    {
        public int FAQConversationId
        {
            get; set;
        }
        public required String Description
        {
            get; set;
        }

        public DateTime CreationDatetime { get; set; }

        public required IdentityUser IdentityUser { get; set; }

        public required String IdentityUserId { get; set; }
    }
}
