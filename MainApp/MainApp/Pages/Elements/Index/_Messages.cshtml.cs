using MainApp.Data;
using MainApp.Models;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Pages.Elements.Index
{
    public class MessagesPageModel : PageModel
    {
        private readonly ApplicationDbContext _db = null!;
        public List<Message> Messages { get; private set; } = [];
        public int ConversationId { get; private set; }
        public int PreviousMessageId { get; private set; }
        public int LastLoadedMessageId { get; private set; }
        
        public MessagesPageModel(ApplicationDbContext db) 
        { 
            _db = db;
        }
        public async Task OnGetAsync(int ConversationId, int? PreviousMessageId)
        {

            // the messages are supposed to be displayed from the bottom to the top
            this.ConversationId = ConversationId;

            var Query = _db.Messages.Where(t => t.ConversationId == ConversationId);
            if (PreviousMessageId != null) {
                this.PreviousMessageId = (int)PreviousMessageId;
                Query = Query.Where(t => t.MessageId < PreviousMessageId); // apply cursor when it's not the first page
            }

            this.Messages = await Query
                .OrderByDescending(t => t.MessageId) // We want the newest message to be the first because of css column reversal shit
                .Take(Consts.MESSAGE_BATCH_SIZE)
                .ToListAsync();

            if(Messages.Count > 0) {
                this.LastLoadedMessageId = Messages.Last().MessageId; // the oldest message id for further loading
            }
            else
            {
                this.LastLoadedMessageId = this.PreviousMessageId;
            }
        }
    }
}
