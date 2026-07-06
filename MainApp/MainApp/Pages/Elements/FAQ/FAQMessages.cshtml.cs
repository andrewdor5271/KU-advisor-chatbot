using MainApp.Data;
using MainApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Pages.Elements.FAQ
{
    public class FAQMessagesModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public FAQMessagesModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public FAQConversation FAQConversation { get; private set; } = null!;
        public List<FAQMessage> Messages { get; private set; } = new();

        public async Task<IActionResult> OnGetAsync(int faqConversationId)
        {
            var conversation = await _db.FAQConversations
                .AsNoTracking()
                .FirstOrDefaultAsync(conversation => conversation.FAQConversationId == faqConversationId);

            if (conversation == null)
            {
                return NotFound();
            }

            FAQConversation = conversation;
            Messages = await _db.FAQMessages
                .AsNoTracking()
                .Where(message => message.FAQConversationId == faqConversationId)
                .OrderBy(message => message.CreationDatetime)
                .ThenBy(message => message.FAQMessageId)
                .ToListAsync();

            return Page();
        }
    }
}
