namespace Reflecta.Models;

/// <summary>Internal mood-analysis result used by Adapter and Strategy patterns.</summary>
public class MoodResult
{
    public MoodLabel Label { get; set; }
    public double Confidence { get; set; }
    public string Summary { get; set; } = string.Empty;
    public List<string> DetectedThemes { get; set; } = new();
}
