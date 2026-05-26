using Reflecta.Models;

namespace Reflecta.Patterns.Behavioral;

public class MoodUpdate
{
    public int          EntryId   { get; set; }
    public MoodLabel    MoodLabel { get; set; }
    public List<string> Themes    { get; set; } = new();
}

/// <summary>Observer interface.</summary>
public interface IMoodObserver
{
    void OnMoodUpdate(MoodUpdate update);
}

public class MoodSubject
{
    private readonly List<IMoodObserver> _observers = new();
    private MoodUpdate? _lastUpdate;

    public void Subscribe(IMoodObserver observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);
    }

    public void Unsubscribe(IMoodObserver observer) =>
        _observers.Remove(observer);

    public void Notify(MoodUpdate update)
    {
        _lastUpdate = update;
        foreach (var obs in _observers.ToList())
            obs.OnMoodUpdate(update);
    }

    public MoodUpdate? GetLastUpdate() => _lastUpdate;
}

public class SummaryChartObserver : IMoodObserver
{
    private readonly List<MoodUpdate> _history = new();
    public IReadOnlyList<MoodUpdate> History => _history.AsReadOnly();

    public event Action? ChartDataChanged;

    public void OnMoodUpdate(MoodUpdate update)
    {
        _history.Add(update);
        ChartDataChanged?.Invoke();
    }
}

public class MoodAlertObserver : IMoodObserver
{
    private readonly Queue<MoodLabel> _recentMoods = new(5);
    public event Action<string>? AlertTriggered;

    public void OnMoodUpdate(MoodUpdate update)
    {
        if (_recentMoods.Count >= 5) _recentMoods.Dequeue();
        _recentMoods.Enqueue(update.MoodLabel);

        var negatives = _recentMoods.Count(m =>
            m is MoodLabel.Sad or MoodLabel.Stressed or MoodLabel.Anxious);

        if (negatives >= 3)
            AlertTriggered?.Invoke(
                "You've been having a tough stretch. Remember to be kind to yourself. 💜");
    }
}
