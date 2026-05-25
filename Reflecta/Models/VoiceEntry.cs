namespace Reflecta.Models;

/// <summary>
/// Concrete product of VoiceEntryFactory (Factory Method pattern).
/// Body stores the transcription; AudioPath the local file URI.
/// </summary>
public class VoiceEntry : JournalEntry
{
    public string AudioPath { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }

    public VoiceEntry()
    {
        Type = EntryType.Voice;
        StateName = "Draft";
    }

    public override string DisplayTitle => $"🎙️ {(string.IsNullOrEmpty(Title) ? "Voice Note" : Title)}";
    public override string DisplayBody  => string.IsNullOrEmpty(Body) ? "(No transcription)" : Body;
}
