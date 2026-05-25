using Reflecta.Models;
using Reflecta.Patterns.Structural;

namespace Reflecta.Patterns.Behavioral;

// ── PATTERN 12: Command ───────────────────────────────────────────────────
// Encapsulates each user action as an object, decoupling the invoker (UI)
// from the receiver (Facade).  DeleteEntryCommand stores the deleted entry
// so it can be restored (Undo support).

/// <summary>Command interface.</summary>
public interface IEntryCommand
{
    Task ExecuteAsync();
    bool CanExecute();
    Task UndoAsync();
}

/// <summary>Save an entry via the facade.</summary>
public class SaveEntryCommand : IEntryCommand
{
    private readonly ReflectaFacade _facade;
    private readonly JournalEntry   _entry;
    private int _savedId;

    public SaveEntryCommand(ReflectaFacade facade, JournalEntry entry)
    {
        _facade = facade;
        _entry  = entry;
    }

    public bool CanExecute() => !string.IsNullOrWhiteSpace(_entry.Body) ||
                                !string.IsNullOrWhiteSpace(_entry.Title);

    public async Task ExecuteAsync()
    {
        _savedId = await _facade.SaveEntryAsync(_entry);
        EntryStateFactory.Transition(_entry, s => s.Save(_entry));
    }

    public Task UndoAsync() => _facade.DeleteEntryAsync(_savedId);
}

/// <summary>Delete an entry and support undo by re-saving.</summary>
public class DeleteEntryCommand : IEntryCommand
{
    private readonly ReflectaFacade _facade;
    private JournalEntry            _deletedEntry;
    private bool                    _executed;

    public DeleteEntryCommand(ReflectaFacade facade, JournalEntry entry)
    {
        _facade       = facade;
        _deletedEntry = entry;
    }

    public bool CanExecute() => _deletedEntry.Id > 0;

    public async Task ExecuteAsync()
    {
        await _facade.DeleteEntryAsync(_deletedEntry.Id);
        _executed = true;
    }

    public async Task UndoAsync()
    {
        if (!_executed) return;
        _deletedEntry.Id = 0; // reset so SQLite inserts rather than updates
        await _facade.SaveEntryAsync(_deletedEntry);
        _executed = false;
    }
}

/// <summary>Export the full journal and return the exported text.</summary>
public class ExportCommand : IEntryCommand
{
    private readonly ReflectaFacade _facade;
    public string ExportedText { get; private set; } = string.Empty;

    public ExportCommand(ReflectaFacade facade) => _facade = facade;

    public bool CanExecute() => true;

    public async Task ExecuteAsync()
    {
        ExportedText = await _facade.ExportJournalAsync();
    }

    public Task UndoAsync() => Task.CompletedTask; // export is read-only
}

/// <summary>
/// Invoker — queues and executes commands, keeping an undo stack.
/// </summary>
public class CommandInvoker
{
    private readonly Stack<IEntryCommand> _undoStack = new();

    public async Task ExecuteAsync(IEntryCommand command)
    {
        if (!command.CanExecute()) return;
        await command.ExecuteAsync();
        _undoStack.Push(command);
    }

    public bool CanUndo => _undoStack.Count > 0;

    public async Task UndoLastAsync()
    {
        if (_undoStack.TryPop(out var command))
            await command.UndoAsync();
    }
}
