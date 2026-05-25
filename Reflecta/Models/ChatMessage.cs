using SQLite;

namespace Reflecta.Models;

[Table("ChatMessages")]
public class ChatMessage
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsUser { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;

    [Ignore]
    public string TimeDisplay => Timestamp.ToString("HH:mm");

    [Ignore]
    public string SenderLabel => IsUser ? "You" : "Reflecta";
}
