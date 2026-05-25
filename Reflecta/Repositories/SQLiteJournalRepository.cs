using Reflecta.Models;
using SQLite;

namespace Reflecta.Repositories;

public class SQLiteJournalRepository : IJournalRepository
{
    private readonly SQLiteAsyncConnection _db;

    public SQLiteJournalRepository(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath,
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
        _db.CreateTableAsync<JournalEntry>().Wait();
    }

    public Task<List<JournalEntry>> GetAllAsync() =>
        _db.Table<JournalEntry>()
           .Where(e => !e.IsArchived)
           .OrderByDescending(e => e.CreatedAt)
           .ToListAsync();

    public async Task<List<JournalEntry>> GetRecentAsync(int days)
    {
        var cutoff = DateTime.Now.AddDays(-days);
        return await _db.Table<JournalEntry>()
            .Where(e => e.CreatedAt >= cutoff && !e.IsArchived)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public Task<JournalEntry?> GetByIdAsync(int id) =>
        _db.Table<JournalEntry>().Where(e => e.Id == id).FirstOrDefaultAsync()!;

    public async Task<int> SaveAsync(JournalEntry entry)
    {
        if (entry.Id == 0)
            await _db.InsertAsync(entry, typeof(JournalEntry));
        else
            await _db.UpdateAsync(entry, typeof(JournalEntry));
        return entry.Id;
    }

    public Task DeleteAsync(int id) =>
        _db.Table<JournalEntry>().DeleteAsync(e => e.Id == id);

    public async Task<List<JournalEntry>> SearchAsync(string query)
    {
        var lower = query.ToLowerInvariant();
        var all   = await GetAllAsync();
        return all.Where(e =>
            e.Title.Contains(lower, StringComparison.OrdinalIgnoreCase) ||
            e.Body.Contains(lower, StringComparison.OrdinalIgnoreCase) ||
            e.Tags.Contains(lower, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
