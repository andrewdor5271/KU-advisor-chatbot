namespace MainApp.Pages.Elements.GlobalPartials
{
    public class ConversationButtonModel
    {
        public String Title { get; set; } = Consts.DUMMY_CONVO_DEFAULT_TITLE;
        public required String HtmlId { get; set; }
        public int? DatabaseId { get; set; } = null;
        public required  String Url { get; set; }
    }
}
