using SQLite;

namespace Reflecta.Models;

public enum EntryType { Text, Mood, Task, Voice }
public enum MoodLabel { Unknown, Happy, Calm, Neutral, Stressed, Sad, Anxious }

/// <summary>
/// Abstract base used by the Factory Method pattern.
/// Concrete subclasses (TextEntry, MoodEntry, TaskEntry, VoiceEntry) add type-specific behaviour.
/// The [Table] / [PrimaryKey] attributes enable SQLite persistence via SQLiteJournalRepository.
/// </summary>
[Table("JournalEntries")]
public class JournalEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public EntryType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty; // comma-separated
    public MoodLabel Mood { get; set; } = MoodLabel.Unknown;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public bool IsPinned { get; set; }
    public bool IsFavorite { get; set; }
    public bool IsEncrypted { get; set; }
    public bool IsArchived { get; set; }

    // ── State pattern hook ──
    [Ignore]
    public string StateName { get; set; } = "Draft";

    public List<string> GetTagList() =>
        Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .Where(t => t.Length > 0)
            .ToList();

    // Used by BindableLayout in JournalPage (computed, not persisted)
    [Ignore]
    public IReadOnlyList<string> TagList => GetTagList();

    public string MoodEmoji => Mood switch
    {
        MoodLabel.Happy   => "😊",
        MoodLabel.Calm    => "😌",
        MoodLabel.Neutral => "😐",
        MoodLabel.Stressed=> "😤",
        MoodLabel.Sad     => "😢",
        MoodLabel.Anxious => "😰",
        _                 => "📝"
    };

    public virtual string DisplayTitle => Title;
    public virtual string DisplayBody  => Body;
}
