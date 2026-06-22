using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Runtime.InteropServices.Swift;

namespace MainApp.Pages
{
    public class IndexPageModel : PageModel
    {
        public int ConversationId { get; private set; }
        public void OnGet(int ConversationID)
        {
            //this.ConversationId = ConversationID;
            this.ConversationId = 1;
        }
    }
}
