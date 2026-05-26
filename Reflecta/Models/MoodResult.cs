namespace Reflecta.Models;

public class MoodResult
{
    public MoodLabel Label { get; set; }
    public double Confidence { get; set; }
    public string Summary { get; set; } = string.Empty;
    public List<string> DetectedThemes { get; set; } = new();
}
