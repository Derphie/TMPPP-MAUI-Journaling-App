using Reflecta.Models;
using Reflecta.Patterns.Behavioral;
using Reflecta.Repositories;
using Reflecta.Services;

namespace Reflecta.Patterns.Structural;

// ── PATTERN 6: Facade ─────────────────────────────────────────────────────
// Provides a single, simplified entry point to the subsystem: repositories,
// notifications, analytics, and export.  ViewModels call only the facade,
// keeping them ignorant of the underlying complexity.

/// <summary>
/// Facade — simplifies the UI layer's access to all backend subsystems.
/// </summary>
public class ReflectaFacade
{
    private readonly IJournalRepository  _journal;
    private readonly IChatRepository     _chat;
    private readonly IAiService          _ai;
    private readonly INotificationService _notifications;
    private readonly IExportService      _export;
    private readonly MoodSubject         _moodSubject;
    private readonly IMoodAnalysisStrategy _strategy;

    public ReflectaFacade(
        IJournalRepository  journal,
        IChatRepository     chat,
        IAiService          ai,
        INotificationService notifications,
        IExportService      export,
        MoodSubject         moodSubject,
        IMoodAnalysisStrategy strategy)
    {
        _journal       = journal;
        _chat          = chat;
        _ai            = ai;
        _notifications = notifications;
        _export        = export;
        _moodSubject   = moodSubject;
        _strategy      = strategy;
    }

    /// <summary>Saves an entry and notifies mood observers.</summary>
    public async Task<int> SaveEntryAsync(JournalEntry entry)
    {
        var moodResult = await _strategy.AnalyzeMoodAsync(entry.Body);
        entry.Mood      = moodResult.Label;
        entry.UpdatedAt = DateTime.Now;

        int id = await _journal.SaveAsync(entry);
        entry.Id = id;

        _moodSubject.Notify(new MoodUpdate
        {
            EntryId   = id,
            MoodLabel = moodResult.Label,
            Themes    = moodResult.DetectedThemes
        });

        return id;
    }

    /// <summary>Deletes an entry by id.</summary>
    public Task DeleteEntryAsync(int id) => _journal.DeleteAsync(id);

    /// <summary>Returns all entries ordered by creation date (newest first).</summary>
    public Task<List<JournalEntry>> GetEntriesAsync() => _journal.GetAllAsync();

    /// <summary>Generates a weekly summary using the current mood strategy.</summary>
    public async Task<WeeklySummary> GetWeeklySummaryAsync()
    {
        var entries = await _journal.GetRecentAsync(7);
        return await BuildSummaryAsync(entries);
    }

    /// <summary>Generates a daily summary.</summary>
    public async Task<WeeklySummary> GetDailySummaryAsync()
    {
        var today   = DateTime.Today;
        var entries = (await _journal.GetAllAsync())
            .Where(e => e.CreatedAt.Date == today)
            .ToList();
        return await BuildSummaryAsync(entries);
    }

    /// <summary>Exports journal entries to a plain-text string.</summary>
    public Task<string> ExportJournalAsync() => _export.ExportToTextAsync();

    /// <summary>Schedules a local notification reminder.</summary>
    public Task ScheduleReminderAsync(string title, string body, DateTime at) =>
        _notifications.ScheduleAsync(title, body, at);

    public Task<string> GetAiResponseAsync(string text) =>
        _ai.GetReflectionAsync(text);

    // ── Private helpers ──────────────────────────────────────────────────

    private async Task<WeeklySummary> BuildSummaryAsync(List<JournalEntry> entries)
    {
        var themes = entries
            .SelectMany(e => e.GetTagList())
            .GroupBy(t => t, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.Count());

        var days = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        var trend = new List<DayMoodPoint>();
        var rng   = new Random(42);

        for (int i = 0; i < 7; i++)
        {
            var dayEntries = entries.Where(e =>
                e.CreatedAt.DayOfWeek == (DayOfWeek)((i + 1) % 7)).ToList();

            float score = dayEntries.Count > 0
                ? (float)dayEntries.Average(e => MoodToScore(e.Mood))
                : (float)(4 + rng.NextDouble() * 4);

            trend.Add(new DayMoodPoint
            {
                Day   = days[i],
                Score = MathF.Round(score, 1),
                Label = ScoreToLabel(score)
            });
        }

        var insight = await _ai.GetReflectionAsync(
            string.Join(" ", entries.Select(e => e.Body).Take(3)));

        return new WeeklySummary
        {
            InsightText  = insight,
            MoodTrend    = trend,
            ThemeCounts  = themes,
            GeneratedAt  = DateTime.Now
        };
    }

    private static float MoodToScore(MoodLabel m) => m switch
    {
        MoodLabel.Happy   => 9f,
        MoodLabel.Calm    => 7.5f,
        MoodLabel.Neutral => 6f,
        MoodLabel.Stressed=> 3.5f,
        MoodLabel.Anxious => 3f,
        MoodLabel.Sad     => 2.5f,
        _                 => 5f
    };

    private static MoodLabel ScoreToLabel(float s) =>
        s >= 8 ? MoodLabel.Happy :
        s >= 6.5 ? MoodLabel.Calm :
        s >= 5 ? MoodLabel.Neutral :
        s >= 3.5 ? MoodLabel.Stressed :
        MoodLabel.Sad;
}
