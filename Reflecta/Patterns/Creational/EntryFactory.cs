using Reflecta.Models;

namespace Reflecta.Patterns.Creational;

// ── PATTERN 1: Factory Method ──────────────────────────────────────────────
// Defines an abstract CreateEntry() hook; each concrete factory produces the
// appropriate JournalEntry subclass.  The caller (JournalViewModel) depends
// only on the abstract factory, keeping entry-creation policy separate from
// the UI.

/// <summary>Abstract creator — declares the factory method.</summary>
public abstract class EntryFactory
{
    public abstract JournalEntry CreateEntry();

    /// <summary>Template method: creates and applies a default title.</summary>
    public JournalEntry CreateWithDefaults(string title = "")
    {
        var entry = CreateEntry();
        if (!string.IsNullOrEmpty(title))
            entry.Title = title;
        entry.CreatedAt = DateTime.Now;
        entry.UpdatedAt = DateTime.Now;
        return entry;
    }
}

/// <summary>Concrete creator — produces TextEntry.</summary>
public class TextEntryFactory : EntryFactory
{
    public override JournalEntry CreateEntry() => new TextEntry();
}

/// <summary>Concrete creator — produces MoodEntry.</summary>
public class MoodEntryFactory : EntryFactory
{
    public override JournalEntry CreateEntry() => new MoodEntry();
}

/// <summary>Concrete creator — produces TaskEntry.</summary>
public class TaskEntryFactory : EntryFactory
{
    public override JournalEntry CreateEntry() => new TaskEntry();
}

/// <summary>Concrete creator — produces VoiceEntry.</summary>
public class VoiceEntryFactory : EntryFactory
{
    public override JournalEntry CreateEntry() => new VoiceEntry();
}

/// <summary>Registry that resolves the correct factory by EntryType.</summary>
public static class EntryFactoryRegistry
{
    private static readonly Dictionary<EntryType, EntryFactory> _map = new()
    {
        [EntryType.Text]  = new TextEntryFactory(),
        [EntryType.Mood]  = new MoodEntryFactory(),
        [EntryType.Task]  = new TaskEntryFactory(),
        [EntryType.Voice] = new VoiceEntryFactory(),
    };

    public static EntryFactory For(EntryType type) => _map[type];

    public static JournalEntry Create(EntryType type, string title = "") =>
        _map[type].CreateWithDefaults(title);
}
