namespace MainApp.Pages.Elements.GlobalPartials
{
    public class ConversationButtonModel
    {
        public String Title { get; set; } = Consts.DUMMY_CONVO_DEFAULT_TITLE;
        public required String Id { get; set; }
        public required  String Url { get; set; }
    }
}
