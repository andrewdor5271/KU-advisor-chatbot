using Microsoft.AspNetCore.Identity;

// this whole namespace describes the data models. These classes are simple structural definitions of the tables
// constraints are defined in the ApplicationDbContext 
namespace MainApp.Models
{
    public class Conversation
    {
        public int ConversationId { get; set; }
        public String Title { get; set; } = null!;

        public DateTime CreationDatetime { get; set; }

        public DateTime LastChangeDatetime { get; set; }

        // either one
        public IdentityUser? IdentityUser { get; private set; }

        public String? IdentityUserId { get; private set; }

        //or the other
        public AnonUser? AnonUser { get; private set; }

        public int? AnonUserId { get; private set; }

        // set corresponding user with checks
        public void SetUserId(String? identityUserId= null, int? anonUserId = null)
        {
            if (identityUserId == null && AnonUser == null)
            {
                throw new ArgumentNullException();
            }

            if (identityUserId != null)
            {
                if (anonUserId != null)
                {
                    // we specifically want this method to only accept one of two args at a time
                    throw new ArgumentException("Ambigous input - both identity and anon user references ids not null");
                }
                this.IdentityUserId = identityUserId;
            }
            else
            {
                this.AnonUserId = anonUserId;
            }
        }

        public void SetUser(IdentityUser? identityUser = null, AnonUser? anonUser = null)
        {
            if (identityUser == null && anonUser == null)
            {
                throw new ArgumentNullException();
            }

            if (identityUser != null)
            {
                if (anonUser != null)
                {
                    // we specifically want this method to only accept one of two args at a time
                    throw new ArgumentException("Ambigous input - both identity and anon user references ids not null");
                }
                this.IdentityUser = identityUser;
            }
            else
            {
                this.AnonUser = anonUser;
            }
        }
    }
}
