using Microsoft.AspNetCore.Identity;

// this whole namespace describes the data models. These classes are simple structural definitions of the tables
// constraints are defined in the ApplicationDbContext 
namespace MainApp.Models
{
    public class Conversation
    {
        private void OnChange()
        {
            this.LastChangeDatetime = DateTime.UtcNow;
        }
        public int ConversationId { get; set
            {
                field = value;
                this.OnChange();
            }
        }
        public String Title { get; set
            {
                field = value;
                this.OnChange();
            }
        } = null!;

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
            if (identityUserId == null && anonUserId == null)
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
                this.AnonUserId = null;
                this.AnonUser = null;
            }
            else
            {
                this.AnonUserId = anonUserId;
                this.IdentityUserId = null;
                this.IdentityUser = null;
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
                this.AnonUserId = null;
                this.AnonUser = null;
            }
            else
            {
                this.AnonUser = anonUser;
                this.IdentityUserId = null;
                this.IdentityUser = null;
            }
            this.OnChange();
        }
    }
}
