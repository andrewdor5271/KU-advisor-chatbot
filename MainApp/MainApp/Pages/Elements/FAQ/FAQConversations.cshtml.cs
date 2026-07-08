using MainApp.Data;
using MainApp.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Pages.Elements.FAQ
{
    public class FAQConversationsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public FAQConversationsModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<FAQConversation> FAQConversations { get; private set; } = new();
        public string? Search { get; private set; }

        public async Task OnGetAsync(string? search)
        {
            Search = search;

            var query = _db.FAQConversations.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var normalizedSearch = search.Trim().ToLower();
                query = query.Where(conversation =>
                    conversation.Description.ToLower().Contains(normalizedSearch) ||
                    _db.FAQMessages.Any(message =>
                        message.FAQConversationId == conversation.FAQConversationId &&
                        message.Text.ToLower().Contains(normalizedSearch)));
            }

            FAQConversations = await query
                .OrderByDescending(conversation => conversation.CreationDatetime)
                .ToListAsync();
        }
    }
}
