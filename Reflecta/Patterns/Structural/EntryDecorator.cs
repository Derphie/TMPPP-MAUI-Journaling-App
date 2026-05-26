using Reflecta.Models;

namespace Reflecta.Patterns.Structural;

public interface IEntry
{
    int      Id              { get; }
    string   Title           { get; }
    string   Body            { get; }
    DateTime CreatedAt       { get; }
    bool     IsPinned        { get; }
    bool     IsFavorite      { get; }
    bool     IsEncrypted     { get; }
    string   GetDisplayTitle();
    string   GetDisplayBody();
    List<string> GetTagList();
}

public class EntryComponentAdapter : IEntry
{
    private readonly JournalEntry _entry;
    public EntryComponentAdapter(JournalEntry entry) => _entry = entry;

    public int      Id          => _entry.Id;
    public string   Title       => _entry.Title;
    public string   Body        => _entry.Body;
    public DateTime CreatedAt   => _entry.CreatedAt;
    public bool     IsPinned    => _entry.IsPinned;
    public bool     IsFavorite  => _entry.IsFavorite;
    public bool     IsEncrypted => _entry.IsEncrypted;

    public string       GetDisplayTitle() => _entry.DisplayTitle;
    public string       GetDisplayBody()  => _entry.DisplayBody;
    public List<string> GetTagList()      => _entry.GetTagList();
}

public abstract class EntryDecorator : IEntry
{
    protected readonly IEntry _inner;
    protected EntryDecorator(IEntry inner) => _inner = inner;

    public virtual int      Id          => _inner.Id;
    public virtual string   Title       => _inner.Title;
    public virtual string   Body        => _inner.Body;
    public virtual DateTime CreatedAt   => _inner.CreatedAt;
    public virtual bool     IsPinned    => _inner.IsPinned;
    public virtual bool     IsFavorite  => _inner.IsFavorite;
    public virtual bool     IsEncrypted => _inner.IsEncrypted;

    public virtual string       GetDisplayTitle() => _inner.GetDisplayTitle();
    public virtual string       GetDisplayBody()  => _inner.GetDisplayBody();
    public virtual List<string> GetTagList()      => _inner.GetTagList();
}

public class PinnedEntry : EntryDecorator
{
    public PinnedEntry(IEntry inner) : base(inner) { }
    public override bool   IsPinned        => true;
    public override string GetDisplayTitle() => "📌 " + _inner.GetDisplayTitle();
}

public class FavoriteEntry : EntryDecorator
{
    public FavoriteEntry(IEntry inner) : base(inner) { }
    public override bool   IsFavorite      => true;
    public override string GetDisplayTitle() => "⭐ " + _inner.GetDisplayTitle();
}

public class EncryptedEntry : EntryDecorator
{
    public EncryptedEntry(IEntry inner) : base(inner) { }
    public override bool   IsEncrypted     => true;
    public override string GetDisplayBody() => "🔒 Content is encrypted";
}

public static class EntryDecoratorFactory
{
    public static IEntry Decorate(JournalEntry entry)
    {
        IEntry result = new EntryComponentAdapter(entry);
        if (entry.IsEncrypted) result = new EncryptedEntry(result);
        if (entry.IsFavorite)  result = new FavoriteEntry(result);
        if (entry.IsPinned)    result = new PinnedEntry(result);
        return result;
    }
}
