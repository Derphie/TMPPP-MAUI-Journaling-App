namespace Reflecta.Models;

/// <summary>
/// Concrete product of TextEntryFactory (Factory Method pattern).
/// A plain text journal entry with optional rich markdown body.
/// </summary>
public class TextEntry : JournalEntry
{
    public TextEntry()
    {
        Type = EntryType.Text;
        StateName = "Draft";
    }

    public override string DisplayTitle => string.IsNullOrEmpty(Title) ? "Text Entry" : Title;
}
