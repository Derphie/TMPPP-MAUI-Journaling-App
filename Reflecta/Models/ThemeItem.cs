namespace Reflecta.Models;

public class ThemeItem
{
    public string Tag   { get; set; } = string.Empty;
    public int    Count { get; set; }
    public string Label => $"{Tag}  {Count}";
}
