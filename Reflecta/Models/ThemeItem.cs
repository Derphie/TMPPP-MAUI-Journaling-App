namespace Reflecta.Models;

/// <summary>A tag-frequency pair for the Summary page's Key Themes display.</summary>
public class ThemeItem
{
    public string Tag   { get; set; } = string.Empty;
    public int    Count { get; set; }
    public string Label => $"{Tag}  {Count}";
}
