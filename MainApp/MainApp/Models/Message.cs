// this whole namespace describes the data models. These classes are simple structural definitions of the tables
// constraints are defined in the ApplicationDbContext 

namespace MainApp.Models
{

    public class Message
    {
        public int MessageId { get; set; }
        public String Text { get; set; } =  null!;

        public DateTime CreationDatetime { get; set; }

        public SenderType SenderType { get; set; }

        public Conversation Conversation { get; set; } = null!;

        public int ConversationId { get; set; }

    }
}
