using Reflecta.Models;

namespace Reflecta.Patterns.Creational;

public abstract class EntryFactory
{
    public abstract JournalEntry CreateEntry();
    
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

public class TextEntryFactory : EntryFactory
{
    public override JournalEntry CreateEntry() => new TextEntry();
}

public class MoodEntryFactory : EntryFactory
{
    public override JournalEntry CreateEntry() => new MoodEntry();
}

public class TaskEntryFactory : EntryFactory
{
    public override JournalEntry CreateEntry() => new TaskEntry();
}

public class VoiceEntryFactory : EntryFactory
{
    public override JournalEntry CreateEntry() => new VoiceEntry();
}

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
