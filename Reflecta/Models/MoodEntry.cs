namespace Reflecta.Models;

public class MoodEntry : JournalEntry
{
    public int MoodIntensity { get; set; } = 5;

    public MoodEntry()
    {
        Type = EntryType.Mood;
        StateName = "Draft";
    }

    public override string DisplayTitle => $"{MoodEmoji} {(string.IsNullOrEmpty(Title) ? "Mood Check-In" : Title)}";
}
