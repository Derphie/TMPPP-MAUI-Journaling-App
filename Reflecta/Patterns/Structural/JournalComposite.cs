namespace Reflecta.Patterns.Structural;

// ── PATTERN 8: Composite ─────────────────────────────────────────────────
// Represents the journal as a tree: Folders contain Notes and other Folders.
// Clients traverse the hierarchy uniformly via IJournalComponent, supporting
// recursive operations like counting entries or flattening for export.

/// <summary>Component — uniform interface for leaves and composites.</summary>
public interface IJournalComponent
{
    string GetTitle();
    int    GetCount();
    void   Add(IJournalComponent component);
    void   Remove(IJournalComponent component);
    IReadOnlyList<IJournalComponent> GetChildren();
    bool   IsLeaf { get; }
    string GetTreeDisplay(int depth = 0);
}

/// <summary>Leaf — a single journal note (wraps an entry ID and title).</summary>
public class JournalNote : IJournalComponent
{
    private readonly int    _entryId;
    private readonly string _title;

    public JournalNote(int entryId, string title)
    {
        _entryId = entryId;
        _title   = title;
    }

    public bool IsLeaf => true;
    public string GetTitle()  => _title;
    public int    GetCount()  => 1;
    public void   Add(IJournalComponent c)    => throw new InvalidOperationException("Leaves have no children.");
    public void   Remove(IJournalComponent c) => throw new InvalidOperationException("Leaves have no children.");
    public IReadOnlyList<IJournalComponent> GetChildren() => Array.Empty<IJournalComponent>();
    public string GetTreeDisplay(int depth = 0) =>
        new string(' ', depth * 2) + $"📄 {_title} [id={_entryId}]";
}

/// <summary>Composite — a folder that holds other components recursively.</summary>
public class JournalFolder : IJournalComponent
{
    private readonly List<IJournalComponent> _children = new();
    public string Name { get; }

    public JournalFolder(string name) => Name = name;

    public bool IsLeaf => false;
    public string GetTitle() => Name;
    public int    GetCount() => _children.Sum(c => c.GetCount());

    public void Add(IJournalComponent component)    => _children.Add(component);
    public void Remove(IJournalComponent component) => _children.Remove(component);
    public IReadOnlyList<IJournalComponent> GetChildren() => _children.AsReadOnly();

    public string GetTreeDisplay(int depth = 0)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine(new string(' ', depth * 2) + $"📁 {Name} ({GetCount()} entries)");
        foreach (var child in _children)
            sb.AppendLine(child.GetTreeDisplay(depth + 1));
        return sb.ToString().TrimEnd();
    }
}

/// <summary>Specialised composite for grouping entries by mood label.</summary>
public class MoodCollection : JournalFolder
{
    public string MoodName { get; }

    public MoodCollection(string moodName) : base($"{moodName} Entries")
    {
        MoodName = moodName;
    }
}
