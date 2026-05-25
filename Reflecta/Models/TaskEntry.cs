namespace Reflecta.Models;

/// <summary>
/// Concrete product of TaskEntryFactory (Factory Method pattern).
/// Tracks tasks or goals alongside reflective notes.
/// </summary>
public class TaskEntry : JournalEntry
{
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }

    public TaskEntry()
    {
        Type = EntryType.Task;
        StateName = "Draft";
    }

    public override string DisplayTitle => (IsCompleted ? "✅ " : "⬜ ") +
                                           (string.IsNullOrEmpty(Title) ? "Task" : Title);
}
