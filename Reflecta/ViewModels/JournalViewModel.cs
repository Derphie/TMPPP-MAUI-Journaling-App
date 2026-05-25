using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Reflecta.Models;
using Reflecta.Patterns.Behavioral;
using Reflecta.Patterns.Creational;
using Reflecta.Patterns.Structural;

namespace Reflecta.ViewModels;

public partial class JournalViewModel : BaseViewModel
{
    private readonly ReflectaFacade    _facade;
    private readonly CommandInvoker    _invoker;
    private readonly TemplateRegistry  _templates;

    public ObservableCollection<JournalEntry> Entries       { get; } = new();
    public ObservableCollection<IEntryPrototype> Templates  { get; } = new();

    [ObservableProperty] private string        _searchQuery = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EditorTitle), nameof(IsNewEntry))]
    private JournalEntry? _selectedEntry;

    public string EditorTitle => SelectedEntry == null ? "New Entry" : "Edit Entry";
    public bool   IsNewEntry  => SelectedEntry == null;

    [ObservableProperty] private string        _newTitle  = string.Empty;
    [ObservableProperty] private string        _newBody   = string.Empty;
    [ObservableProperty] private string        _newTags   = string.Empty;
    [ObservableProperty] private int           _newTypeIndex; // 0=Text,1=Mood,2=Task,3=Voice
    [ObservableProperty] private bool          _showEditor;

    public JournalViewModel(ReflectaFacade facade)
    {
        _facade    = facade;
        _invoker   = new CommandInvoker();
        _templates = new TemplateRegistry();
        Title      = "Journal";

        foreach (var t in _templates.GetAll())
            Templates.Add(t);
    }

    public async Task InitializeAsync()
    {
        await RunSafeAsync(async () =>
        {
            var entries = await _facade.GetEntriesAsync();
            Entries.Clear();
            foreach (var e in entries) Entries.Add(e);
        });
    }

    // ── Search ──────────────────────────────────────────────────────────

    partial void OnSearchQueryChanged(string value) =>
        _ = SearchAsync(value);

    private async Task SearchAsync(string query)
    {
        await RunSafeAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                await InitializeAsync();
                return;
            }
            var result = await _facade.GetEntriesAsync();
            var filtered = result.Where(e =>
                e.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                e.Body.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                e.Tags.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
            Entries.Clear();
            foreach (var e in filtered) Entries.Add(e);
        });
    }

    // ── New entry ────────────────────────────────────────────────────────

    [RelayCommand]
    private void OpenNewEntry()
    {
        NewTitle     = string.Empty;
        NewBody      = string.Empty;
        NewTags      = string.Empty;
        NewTypeIndex = 0;
        SelectedEntry = null;
        ShowEditor   = true;
    }

    [RelayCommand]
    private void OpenEntry(JournalEntry entry)
    {
        SelectedEntry = entry;
        NewTitle      = entry.Title;
        NewBody       = entry.Body;
        NewTags       = entry.Tags;
        NewTypeIndex  = (int)entry.Type;
        ShowEditor    = true;
    }

    [RelayCommand]
    private async Task SaveEntryAsync()
    {
        var type  = (EntryType)NewTypeIndex;
        var entry = SelectedEntry ?? EntryFactoryRegistry.Create(type);

        entry.Title     = NewTitle;
        entry.Body      = NewBody;
        entry.Tags      = NewTags;
        entry.Type      = type;
        entry.UpdatedAt = DateTime.Now;
        if (entry.StateName == "Draft") entry.StateName = "Editing";

        var cmd = new SaveEntryCommand(_facade, entry);
        await _invoker.ExecuteAsync(cmd);

        ShowEditor = false;
        await InitializeAsync();
    }

    [RelayCommand]
    private async Task DeleteEntryAsync(JournalEntry entry)
    {
        var cmd = new DeleteEntryCommand(_facade, entry);
        await _invoker.ExecuteAsync(cmd);
        Entries.Remove(entry);

        var snackbar = Snackbar.Make(
            "Entry deleted — Undo",
            async () => { await _invoker.UndoLastAsync(); await InitializeAsync(); },
            "Undo",
            TimeSpan.FromSeconds(4));
        await snackbar.Show();
    }

    [RelayCommand]
    private async Task UndoLastAsync()
    {
        if (_invoker.CanUndo)
        {
            await _invoker.UndoLastAsync();
            await InitializeAsync();
        }
    }

    /// <summary>
    /// Opens a Material bottom-sheet action menu for the given entry.
    /// Uses Command pattern (DeleteEntryCommand) and Decorator pattern (TogglePinAsync).
    /// </summary>
    [RelayCommand]
    private async Task ShowEntryMenuAsync(JournalEntry entry)
    {
        string? action = await Shell.Current.DisplayActionSheet(
            null,
            "Cancel",
            null,
            entry.IsPinned ? "📌 Unpin" : "📌 Pin",
            "🗑️ Delete");

        if (action is "📌 Pin" or "📌 Unpin")
        {
            await TogglePinAsync(entry);
        }
        else if (action == "🗑️ Delete")
        {
            bool confirmed = await Shell.Current.DisplayAlert(
                "Delete Entry",
                "Delete this journal entry? You can undo right after.",
                "Delete",
                "Cancel");
            if (confirmed)
                await DeleteEntryAsync(entry);
        }
    }

    [RelayCommand]
    private async Task UseTemplateAsync(IEntryPrototype template)
    {
        var cloned    = template.Clone();
        NewTitle      = cloned.Title;
        NewBody       = cloned.Body;
        NewTags       = cloned.Tags;
        NewTypeIndex  = (int)cloned.Type;
        SelectedEntry = null;
        ShowEditor    = true;
    }

    [RelayCommand]
    private async Task TogglePinAsync(JournalEntry entry)
    {
        await _facade.TogglePinAsync(entry);
        await InitializeAsync();
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync(JournalEntry entry)
    {
        entry.IsFavorite = !entry.IsFavorite;
        await _facade.SaveEntryAsync(entry);
        await InitializeAsync();
    }

    [RelayCommand]
    private void DismissEditor() => ShowEditor = false;
}
