using Reflecta.Models;

namespace Reflecta.Patterns.Creational;

// ── PATTERN 2: Builder ────────────────────────────────────────────────────
// Separates the construction of a complex JournalEntry object from its
// representation. The fluent interface lets the "new entry" flow and
// Prototype templates compose entries step-by-step.

/// <summary>
/// Fluent builder for JournalEntry. Ensures all optional fields are set in a
/// controlled order before Build() produces the final object.
/// </summary>
public class JournalEntryBuilder
{
    private readonly JournalEntry _entry;

    public JournalEntryBuilder(EntryType type = EntryType.Text)
    {
        _entry = EntryFactoryRegistry.Create(type);
    }

    public JournalEntryBuilder WithTitle(string title)
    {
        _entry.Title = title;
        return this;
    }

    public JournalEntryBuilder WithBody(string body)
    {
        _entry.Body = body;
        return this;
    }

    public JournalEntryBuilder WithTags(params string[] tags)
    {
        _entry.Tags = string.Join(",", tags);
        return this;
    }

    public JournalEntryBuilder WithMood(MoodLabel mood)
    {
        _entry.Mood = mood;
        return this;
    }

    public JournalEntryBuilder WithPinned(bool pinned = true)
    {
        _entry.IsPinned = pinned;
        return this;
    }

    public JournalEntryBuilder WithFavorite(bool favorite = true)
    {
        _entry.IsFavorite = favorite;
        return this;
    }

    public JournalEntryBuilder WithEncryption(bool encrypted = true)
    {
        _entry.IsEncrypted = encrypted;
        return this;
    }

    public JournalEntryBuilder WithReminder(DateTime? reminderAt = null)
    {
        // stored via notification service after save
        return this;
    }

    public JournalEntryBuilder WithMetadata(DateTime createdAt)
    {
        _entry.CreatedAt = createdAt;
        _entry.UpdatedAt = createdAt;
        return this;
    }

    public JournalEntry Build()
    {
        _entry.UpdatedAt = DateTime.Now;
        return _entry;
    }
}
