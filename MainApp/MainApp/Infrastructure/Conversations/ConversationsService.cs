using MainApp.Data;
using MainApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Infrastructure.Conversations
{
    public class ConversationsService : IConversationsService
    {
        private readonly ApplicationDbContext _db;

        public ConversationsService(ApplicationDbContext db)
        {
            this._db = db;
        }

        public async Task<Conversation> CreateForAnonAsync(int anonId, String title)
        {
            var creationTime = DateTime.UtcNow;
            var convo = new Conversation
            {
                Title = title,
                CreationDatetime = creationTime,
                LastChangeDatetime = creationTime
            };
            convo.SetUserId(null, anonId);
            this._db.Conversations.Add(convo);
            await this._db.SaveChangesAsync();
            return convo;
        }
        public async Task<Conversation> CreateForIdentityAsync(String identityId, String title)
        {
            var creationTime = DateTime.UtcNow;
            var convo = new Conversation
            {
                Title = title,
                CreationDatetime = creationTime,
                LastChangeDatetime = creationTime
            };
            convo.SetUserId(identityId, null);
            this._db.Conversations.Add(convo);
            await this._db.SaveChangesAsync();
            return convo;
        }

        public async Task DeleteAsync(int conversationId)
        {
            await this._db.Conversations
                .Where(t => t.ConversationId == conversationId)
                .ExecuteDeleteAsync();
            await this._db.SaveChangesAsync();
        }

        public async Task<Conversation?> GetAsync(int conversationId)
        {
            return await this._db.Conversations
                .SingleAsync(t => t.ConversationId == conversationId);
        }

        public async Task<List<Conversation>> GetForAnonUserAsync(int anonId)
        {
            return await this._db.Conversations
                .Where(t => t.AnonUserId == anonId)
                .OrderByDescending(t => t.ConversationId)
                .ToListAsync();
        }

        public async Task<List<Conversation>> GetForIdentityUserAsync(String identityId)
        {
            return await this._db.Conversations
                .Where(t => t.IdentityUserId == identityId)
                .OrderByDescending(t => t.ConversationId)
                .ToListAsync();
        }

        public async Task RenameAsync(int conversationId, string newTitle)
        {
            var convo = await this._db.Conversations.
                SingleAsync (t => t.ConversationId == conversationId);
            convo.Title = newTitle;
            _db.Conversations.Add(convo);
            await this._db.SaveChangesAsync();
        }

        public async Task UpdateLastUsageAsync(int conversationId)
        {
            var convo = await this._db.Conversations.
                SingleAsync(t => t.ConversationId == conversationId);
            convo.LastChangeDatetime = DateTime.UtcNow;
            _db.Conversations.Add(convo);
            await this._db.SaveChangesAsync();
        }
    }
}
