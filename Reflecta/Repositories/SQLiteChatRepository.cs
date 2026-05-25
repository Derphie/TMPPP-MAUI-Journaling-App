using Reflecta.Models;
using SQLite;

namespace Reflecta.Repositories;

public class SQLiteChatRepository : IChatRepository
{
    private readonly SQLiteAsyncConnection _db;

    public SQLiteChatRepository(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath,
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
        _db.CreateTableAsync<ChatMessage>().Wait();
    }

    public Task<List<ChatMessage>> GetAllAsync() =>
        _db.Table<ChatMessage>().OrderBy(m => m.Timestamp).ToListAsync();

    public Task SaveMessageAsync(ChatMessage message) =>
        message.Id == 0 ? _db.InsertAsync(message) : _db.UpdateAsync(message);

    public Task ClearAsync() => _db.DeleteAllAsync<ChatMessage>();
}
