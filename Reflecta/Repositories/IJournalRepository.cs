using Reflecta.Models;

namespace Reflecta.Repositories;

public interface IJournalRepository
{
    Task<List<JournalEntry>> GetAllAsync();
    Task<List<JournalEntry>> GetRecentAsync(int days);
    Task<JournalEntry?> GetByIdAsync(int id);
    Task<int> SaveAsync(JournalEntry entry);
    Task DeleteAsync(int id);
    Task<List<JournalEntry>> SearchAsync(string query);
}
