using MainApp.Data;
using MainApp.Infrastructure.Authentication;
using MainApp.Infrastructure.Page;
using MainApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Pages.Elements.Index
{
    [BimodalAuthentify]
    public class MessageBatchPageModel : BimodalAuthMessagePageModel
    {
        public List<Message> Messages { get; private set; } = null!;
        public int ConversationId { get; private set; }
        public int? LastLoadedMessageId { get; private set; } = null;
        public MessageBatchPageModel(ApplicationDbContext db) : base(db)
        {
        }

        public async Task<IActionResult> OnGetAsync(int conversationId, int? previousMessageId)
        {
            if(!await this.CheckMessagesSecurityAsync(conversationId))
            {
                return Forbid();
            }

            this.ConversationId = conversationId;

            var Query = _db.Messages.Where(t => t.ConversationId == conversationId);
            if (previousMessageId != null)
            {
                Query = Query.Where(t => t.MessageId < previousMessageId); // apply cursor when it's not the first page
            }

            this.Messages = await Query
                .OrderByDescending(t => t.MessageId) // We want the newest message to be the first because of css column reversal shit
                .Take(Consts.MESSAGE_BATCH_SIZE)
                .ToListAsync();

            if (Messages.Count > 0)
            {
                this.LastLoadedMessageId = Messages.Last().MessageId;
            }

            return Page();
        }
    }
}
