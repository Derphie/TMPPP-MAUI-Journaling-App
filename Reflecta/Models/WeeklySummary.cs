namespace Reflecta.Models;

public class DayMoodPoint
{
    public string Day { get; set; } = string.Empty;
    public float Score { get; set; } // 0–10
    public MoodLabel Label { get; set; }
}

public class WeeklySummary
{
    public string InsightText { get; set; } = string.Empty;
    public List<DayMoodPoint> MoodTrend { get; set; } = new();
    public Dictionary<string, int> ThemeCounts { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.Now;
}
