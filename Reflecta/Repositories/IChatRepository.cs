using Reflecta.Models;

namespace Reflecta.Repositories;

public interface IChatRepository
{
    Task<List<ChatMessage>> GetAllAsync();
    Task SaveMessageAsync(ChatMessage message);
    Task ClearAsync();
}
