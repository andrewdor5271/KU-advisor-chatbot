using MainApp.Infrastructure.Authentication;
using MainApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MainApp.Pages.Elements.Index
{
    [BimodalAuthentify]
    public class ConversationsPageModel : PageModel
    {
        public List<Conversation> Conversations { get; private set; } = null!;
        public async Task OnGetAsync(int conversationId)
        {

        }
    }
}
