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

        public IdentityUser User { get; set; } = null!;

        public String UserId { get; set; } = null!;

    }
}
