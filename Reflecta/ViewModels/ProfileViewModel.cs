using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Reflecta.Models;
using Reflecta.Patterns.Behavioral;
using Reflecta.Patterns.Structural;
using Reflecta.Repositories;
using Reflecta.Services;
using SQLite;

namespace Reflecta.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly ReflectaFacade    _facade;
    private readonly IJournalRepository _journal;
    private readonly INotificationService _notifications;

    [ObservableProperty] private string _userName     = "Journaler";
    [ObservableProperty] private string _avatarEmoji  = "🌟";
    [ObservableProperty] private bool   _notificationsEnabled = true;
    [ObservableProperty] private string _reminderTime = "21:00";
    [ObservableProperty] private int    _strategyIndex = 0; 
    [ObservableProperty] private int    _totalEntries;
    [ObservableProperty] private int    _currentStreak;
    [ObservableProperty] private string _memberSince = string.Empty;

    public string[] StrategyNames { get; } = ["Simple (Offline)", "AI Weighted"];

    public ProfileViewModel(ReflectaFacade facade, IJournalRepository journal,
                            INotificationService notifications)
    {
        _facade        = facade;
        _journal       = journal;
        _notifications = notifications;
        Title          = "Profile";
    }

    public async Task InitializeAsync()
    {
        await RunSafeAsync(async () =>
        {
            var entries = await _journal.GetAllAsync();
            TotalEntries = entries.Count;
            MemberSince  = entries.Count > 0
                ? entries.Min(e => e.CreatedAt).ToString("MMMM yyyy")
                : DateTime.Now.ToString("MMMM yyyy");
            
            var dates = entries.Select(e => e.CreatedAt.Date).Distinct().OrderByDescending(d => d).ToList();
            int streak = 0;
            var check  = DateTime.Today;
            foreach (var d in dates)
            {
                if (d == check) { streak++; check = check.AddDays(-1); }
                else break;
            }
            CurrentStreak = streak;
        });
    }

    [RelayCommand]
    private async Task SaveReminderAsync()
    {
        if (!NotificationsEnabled) return;
        if (!TimeSpan.TryParse(ReminderTime, out var time)) return;

        var at = DateTime.Today.Add(time);
        if (at < DateTime.Now) at = at.AddDays(1);

        await _facade.ScheduleReminderAsync(
            "Time to reflect ✦",
            "How was your day? Open Reflecta and write a few thoughts.",
            at);
    }

    [RelayCommand]
    private async Task ExportJournalAsync()
    {
        var cmd = new ExportCommand(_facade);
        await cmd.ExecuteAsync();
        await Share.RequestAsync(new ShareTextRequest
        {
            Title = "My Reflecta Journal",
            Text  = cmd.ExportedText
        });
    }
}
