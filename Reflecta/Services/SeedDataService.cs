using Reflecta.Models;
using Reflecta.Patterns.Creational;
using Reflecta.Repositories;

namespace Reflecta.Services;

/// <summary>Seeds mock data so all screens are populated on first launch.</summary>
public class SeedDataService
{
    private readonly IJournalRepository _journal;
    private readonly IChatRepository    _chat;

    public SeedDataService(IJournalRepository journal, IChatRepository chat)
    {
        _journal = journal;
        _chat    = chat;
    }

    public async Task SeedAsync()
    {
        var existingEntries = await _journal.GetAllAsync();
        if (existingEntries.Count > 0) return; // already seeded

        var now = DateTime.Now;

        var entries = new[]
        {
            new JournalEntryBuilder(EntryType.Text)
                .WithTitle("Finals Week Survival")
                .WithBody("So stressed about finals. I have three exams back-to-back and I haven't slept properly in days. " +
                          "I keep telling myself I'll be okay but the anxiety is real. Need to break this study block somehow.")
                .WithTags("school", "stress", "exams")
                .WithMood(MoodLabel.Stressed)
                .WithMetadata(now.AddDays(-6))
                .Build(),

            new JournalEntryBuilder(EntryType.Mood)
                .WithTitle("Morning Check-In")
                .WithBody("Feeling a bit better after my morning run. The fresh air helped. Still worried about the presentation " +
                          "but my mind is clearer. Grateful for small wins.")
                .WithTags("morning", "exercise", "grateful")
                .WithMood(MoodLabel.Calm)
                .WithMetadata(now.AddDays(-5))
                .Build(),

            new JournalEntryBuilder(EntryType.Text)
                .WithTitle("Gratitude Entry")
                .WithBody("Things I'm grateful for today:\n1. My supportive friends who checked in on me\n" +
                          "2. Getting 7 hours of sleep last night\n3. A sunny afternoon that lifted my mood\n" +
                          "4. Finally understanding that calculus concept")
                .WithTags("gratitude", "positivity", "reflection")
                .WithMood(MoodLabel.Happy)
                .WithMetadata(now.AddDays(-4))
                .Build(),

            new JournalEntryBuilder(EntryType.Task)
                .WithTitle("Weekly Goals")
                .WithBody("Goal 1: Finish all assignments by Thursday\n  Action: 2-hour study blocks each morning\n\n" +
                          "Goal 2: Work out 3 times this week\n  Action: Morning runs Mon/Wed/Fri\n\n" +
                          "Goal 3: Meditate for 10 minutes daily\n  Action: Right before bed")
                .WithTags("goals", "planning", "weekly")
                .WithMood(MoodLabel.Neutral)
                .WithMetadata(now.AddDays(-3))
                .Build(),

            new JournalEntryBuilder(EntryType.Text)
                .WithTitle("Post-Exam Relief")
                .WithBody("The exam actually went well! I can't believe it. All that studying paid off. " +
                          "Went out with friends to celebrate — first time I've properly laughed in weeks. " +
                          "Feeling lighter, like a weight has been lifted.")
                .WithTags("school", "celebration", "happy")
                .WithMood(MoodLabel.Happy)
                .WithMetadata(now.AddDays(-2))
                .Build(),

            new JournalEntryBuilder(EntryType.Mood)
                .WithTitle("Sunday Reflection")
                .WithBody("A quiet Sunday. Read a book for two hours without checking my phone — that almost never happens. " +
                          "Feeling introspective. What do I really want to focus on next month? Maybe start drawing again.")
                .WithTags("reflection", "calm", "goals")
                .WithMood(MoodLabel.Calm)
                .WithPinned()
                .WithMetadata(now.AddDays(-1))
                .Build(),

            new JournalEntryBuilder(EntryType.Text)
                .WithTitle("Today's Thoughts")
                .WithBody("New week, fresh start. Had a productive morning — got through my inbox, " +
                          "drafted my essay outline, and actually ate breakfast. Small progress is still progress. " +
                          "Trying to stay present and not catastrophise about the future.")
                .WithTags("productivity", "mindset", "school")
                .WithMood(MoodLabel.Neutral)
                .WithFavorite()
                .WithMetadata(now)
                .Build(),
        };

        foreach (var e in entries)
            await _journal.SaveAsync(e);

        // Seed chat messages
        var messages = new[]
        {
            new ChatMessage { Text = "Hey Reflecta, I've been so stressed about finals lately.", IsUser = true,
                              Timestamp = now.AddDays(-2).AddHours(-1) },
            new ChatMessage { Text = "It sounds like you're carrying a lot right now. Stress can feel very heavy — especially when deadlines stack up. " +
                                     "Try breaking your workload into smaller pieces and take a short walk between tasks. " +
                                     "What's the single most important thing you could cross off your list today?",
                              IsUser = false, Timestamp = now.AddDays(-2).AddHours(-1).AddMinutes(1) },
            new ChatMessage { Text = "That's really helpful. I think I'll start with the essay since that's due first.", IsUser = true,
                              Timestamp = now.AddDays(-2).AddHours(-1).AddMinutes(3) },
            new ChatMessage { Text = "That's a great choice — starting with the most time-sensitive task gives you immediate momentum. " +
                                     "Would you like some tips on beating writer's block, or do you feel ready to dive in?",
                              IsUser = false, Timestamp = now.AddDays(-2).AddHours(-1).AddMinutes(4) },
            new ChatMessage { Text = "How are you today?", IsUser = true,
                              Timestamp = now.AddMinutes(-5) },
            new ChatMessage { Text = "Thank you for sharing that with me. Journaling like this is a genuine act of self-care. " +
                                     "How are you feeling right now, in this moment? 💜",
                              IsUser = false, Timestamp = now.AddMinutes(-4) },
        };

        foreach (var m in messages)
            await _chat.SaveMessageAsync(m);
    }
}
