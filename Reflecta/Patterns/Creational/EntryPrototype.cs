using Reflecta.Models;

namespace Reflecta.Patterns.Creational;

// ── PATTERN 4: Prototype ─────────────────────────────────────────────────
// Allows cloning pre-configured entry templates without going through the
// factory chain.  The user picks a template (e.g. "Daily Reflection") and
// the app deep-copies it, yielding a ready-to-edit entry with prefilled
// structure and tags.

/// <summary>Prototype interface — all templates implement this.</summary>
public interface IEntryPrototype
{
    string TemplateName { get; }
    string Description  { get; }
    JournalEntry Clone();
}

/// <summary>Daily reflection template — pre-fills prompts and tags.</summary>
public class DailyReflectionTemplate : IEntryPrototype
{
    public string TemplateName => "Daily Reflection";
    public string Description  => "Three gratitudes, one challenge, one intention";

    public JournalEntry Clone() => new JournalEntryBuilder(EntryType.Text)
        .WithTitle("Daily Reflection")
        .WithBody("✦ Three things I'm grateful for:\n1. \n2. \n3. \n\n" +
                  "✦ Today's challenge:\n\n" +
                  "✦ Tomorrow's intention:\n")
        .WithTags("reflection", "gratitude", "daily")
        .WithMood(MoodLabel.Neutral)
        .WithMetadata(DateTime.Now)
        .Build();
}

/// <summary>Gratitude journal template — pure positive focus.</summary>
public class GratitudeTemplate : IEntryPrototype
{
    public string TemplateName => "Gratitude Journal";
    public string Description  => "Five gratitudes with brief context";

    public JournalEntry Clone() => new JournalEntryBuilder(EntryType.Text)
        .WithTitle("Gratitude Journal")
        .WithBody("Today I am grateful for:\n1. \n2. \n3. \n4. \n5. \n\n" +
                  "The highlight of my day was:\n")
        .WithTags("gratitude", "positivity")
        .WithMood(MoodLabel.Happy)
        .WithMetadata(DateTime.Now)
        .Build();
}

/// <summary>Stress check-in template for difficult days.</summary>
public class StressCheckInTemplate : IEntryPrototype
{
    public string TemplateName => "Stress Check-In";
    public string Description  => "Structured venting + coping strategy";

    public JournalEntry Clone() => new JournalEntryBuilder(EntryType.Mood)
        .WithTitle("Stress Check-In")
        .WithBody("What's bothering me right now:\n\n" +
                  "Why it matters to me:\n\n" +
                  "One thing I can control:\n\n" +
                  "Self-compassion note:\n")
        .WithTags("stress", "coping", "selfcare")
        .WithMood(MoodLabel.Stressed)
        .WithMetadata(DateTime.Now)
        .Build();
}

/// <summary>Weekly goals template for Sunday planning.</summary>
public class WeeklyGoalsTemplate : IEntryPrototype
{
    public string TemplateName => "Weekly Goals";
    public string Description  => "3 goals with actions and accountability";

    public JournalEntry Clone() => new JournalEntryBuilder(EntryType.Task)
        .WithTitle("Weekly Goals")
        .WithBody("Goal 1:\n  Actions:\n\nGoal 2:\n  Actions:\n\nGoal 3:\n  Actions:\n\n" +
                  "How I'll hold myself accountable:\n")
        .WithTags("goals", "planning", "weekly")
        .WithMood(MoodLabel.Calm)
        .WithMetadata(DateTime.Now)
        .Build();
}

/// <summary>In-memory registry of all available templates.</summary>
public class TemplateRegistry
{
    private static readonly List<IEntryPrototype> _templates = new()
    {
        new DailyReflectionTemplate(),
        new GratitudeTemplate(),
        new StressCheckInTemplate(),
        new WeeklyGoalsTemplate(),
    };

    public IReadOnlyList<IEntryPrototype> GetAll() => _templates.AsReadOnly();

    public IEntryPrototype? Find(string name) =>
        _templates.FirstOrDefault(t => t.TemplateName == name);
}
