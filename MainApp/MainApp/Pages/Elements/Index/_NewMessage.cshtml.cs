using MainApp.Data;
using MainApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MainApp.Pages.Elements.Index
{
    public class NewMessagePageModel : PageModel
    {
        private ApplicationDbContext _db;
        public Message Message { get; private set; } = null!;
        public NewMessagePageModel(ApplicationDbContext db)
        { 
            this._db = db;
        }
        public async Task OnPostAsync(int ConversationId, String Text)
        {
            Console.WriteLine("Handler check CAUGHT");
            this.Message = new Message
            {
                ConversationId = ConversationId,
                Text = Text,
                CreationDatetime = DateTime.Now
            };

            this._db.Add(this.Message);
            await this._db.SaveChangesAsync();
        }
}
}
