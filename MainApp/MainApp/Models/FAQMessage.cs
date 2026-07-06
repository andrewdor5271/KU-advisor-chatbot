namespace MainApp.Models
{
    public class FAQMessage
    {
        public int FAQMessageId { get; set; }
        public required String Text { get; set; }

        public DateTime CreationDatetime { get; set; }

        public SenderType SenderType { get; set; }

        public required FAQConversation FAQConversation { get; set; }

        public int FAQConversationId { get; set; }
    }
}
