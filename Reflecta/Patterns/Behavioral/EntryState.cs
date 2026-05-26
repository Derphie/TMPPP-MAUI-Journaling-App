using Reflecta.Models;

namespace Reflecta.Patterns.Behavioral;

public interface IEntryState
{
    string Name { get; }
    void BeginEdit(JournalEntry entry);
    void Save(JournalEntry entry);
    void Archive(JournalEntry entry);
    void Restore(JournalEntry entry);
}

public class DraftState : IEntryState
{
    public string Name => "Draft";

    public void BeginEdit(JournalEntry entry)
    {
        entry.StateName = "Editing";
    }

    public void Save(JournalEntry entry)
    {
        entry.UpdatedAt = DateTime.Now;
        entry.StateName = "Saved";
    }

    public void Archive(JournalEntry entry) =>
        throw new InvalidOperationException("Cannot archive an unsaved draft.");

    public void Restore(JournalEntry entry) =>
        throw new InvalidOperationException("Draft has not been archived.");
}

public class EditingState : IEntryState
{
    public string Name => "Editing";

    public void BeginEdit(JournalEntry entry) { /* already editing — no-op */ }

    public void Save(JournalEntry entry)
    {
        entry.UpdatedAt = DateTime.Now;
        entry.StateName = "Saved";
    }

    public void Archive(JournalEntry entry) =>
        throw new InvalidOperationException("Save before archiving.");

    public void Restore(JournalEntry entry) =>
        throw new InvalidOperationException("Entry is being edited.");
}

public class SavedState : IEntryState
{
    public string Name => "Saved";

    public void BeginEdit(JournalEntry entry)
    {
        entry.StateName = "Editing";
    }

    public void Save(JournalEntry entry) { /* idempotent re-save */ }

    public void Archive(JournalEntry entry)
    {
        entry.IsArchived = true;
        entry.StateName  = "Archived";
    }

    public void Restore(JournalEntry entry) =>
        throw new InvalidOperationException("Entry is not archived.");
}

public class ArchivedState : IEntryState
{
    public string Name => "Archived";

    public void BeginEdit(JournalEntry entry) =>
        throw new InvalidOperationException("Restore the entry before editing.");

    public void Save(JournalEntry entry) =>
        throw new InvalidOperationException("Cannot save an archived entry.");

    public void Archive(JournalEntry entry) { /* already archived — no-op */ }

    public void Restore(JournalEntry entry)
    {
        entry.IsArchived = false;
        entry.StateName  = "Saved";
    }
}

public static class EntryStateFactory
{
    public static IEntryState Resolve(JournalEntry entry) => entry.StateName switch
    {
        "Draft"    => new DraftState(),
        "Editing"  => new EditingState(),
        "Saved"    => new SavedState(),
        "Archived" => new ArchivedState(),
        _          => new DraftState()
    };

    public static void Transition(JournalEntry entry, Action<IEntryState> action)
    {
        var state = Resolve(entry);
        action(state);
    }
}
