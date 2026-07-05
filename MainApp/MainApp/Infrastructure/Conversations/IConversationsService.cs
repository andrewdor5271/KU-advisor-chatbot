using MainApp.Models;

namespace MainApp.Infrastructure.Conversations
{
    // SERVICE UNSECURE. SECURITY CHECKS AT THE PAGES
    public interface IConversationsService
    {
        Task<Conversation> CreateForAnonAsync(int anonId, String title);
        Task<Conversation> CreateForIdentityAsync(String identityId, String title);

        Task<Conversation?> GetAsync(int conversationId);
        Task AddAsync(Conversation convo);

        Task<List<Conversation>> GetForAnonUserAsync(int anonId);
        Task<List<Conversation>> GetForIdentityUserAsync(String identityId);

        Task RenameAsync(int conversationId, string newTitle);

        Task DeleteAsync(int conversationId);

        Task UpdateLastUsageAsync(int conversationId);
    }
}
