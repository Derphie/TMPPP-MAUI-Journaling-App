namespace Reflecta.Models;

/// <summary>
/// Concrete product of MoodEntryFactory (Factory Method pattern).
/// Specialised for quick mood check-ins; Body holds a short note.
/// </summary>
public class MoodEntry : JournalEntry
{
    public int MoodIntensity { get; set; } = 5; // 1–10

    public MoodEntry()
    {
        Type = EntryType.Mood;
        StateName = "Draft";
    }

    public override string DisplayTitle => $"{MoodEmoji} {(string.IsNullOrEmpty(Title) ? "Mood Check-In" : Title)}";
}
