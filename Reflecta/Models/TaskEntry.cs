namespace Reflecta.Models;

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
