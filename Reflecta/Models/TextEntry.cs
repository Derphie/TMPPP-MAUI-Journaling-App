namespace Reflecta.Models;

public class TextEntry : JournalEntry
{
    public TextEntry()
    {
        Type = EntryType.Text;
        StateName = "Draft";
    }

    public override string DisplayTitle => string.IsNullOrEmpty(Title) ? "Text Entry" : Title;
}
