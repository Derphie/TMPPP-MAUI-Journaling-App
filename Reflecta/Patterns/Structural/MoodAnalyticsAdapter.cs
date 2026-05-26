using Reflecta.Models;

namespace Reflecta.Patterns.Structural;


public class ExternalMoodApiResponse
{
    public string emotion_code { get; set; } = "neutral";
    public double confidence_pct { get; set; } = 50.0;
    public string[] keywords { get; set; } = Array.Empty<string>();
    public string raw_summary { get; set; } = string.Empty;
}

public class ExternalMoodApiClient
{
    public Task<ExternalMoodApiResponse> AnalyzeAsync(string text)
    {
        var lower = text.ToLowerInvariant();
        var response = new ExternalMoodApiResponse
        {
            raw_summary = $"External analysis of {text.Length} chars"
        };

        if (lower.Contains("stress") || lower.Contains("anxious") || lower.Contains("overwhelm"))
        {
            response.emotion_code    = "stressed";
            response.confidence_pct = 85.0;
            response.keywords       = new[] { "stress", "pressure" };
        }
        else if (lower.Contains("happy") || lower.Contains("great") || lower.Contains("wonderful"))
        {
            response.emotion_code    = "happy";
            response.confidence_pct = 90.0;
            response.keywords       = new[] { "joy", "positivity" };
        }
        else if (lower.Contains("sad") || lower.Contains("depressed") || lower.Contains("down"))
        {
            response.emotion_code    = "sad";
            response.confidence_pct = 80.0;
            response.keywords       = new[] { "sadness", "low energy" };
        }
        else if (lower.Contains("tired") || lower.Contains("exhausted"))
        {
            response.emotion_code    = "fatigued";
            response.confidence_pct = 75.0;
            response.keywords       = new[] { "fatigue", "rest" };
        }
        else
        {
            response.emotion_code    = "neutral";
            response.confidence_pct = 60.0;
            response.keywords       = new[] { "neutral" };
        }

        return Task.FromResult(response);
    }
}


public interface IMoodAnalyzer
{
    Task<MoodResult> AnalyzeAsync(string text);
}

public class MoodAnalyticsAdapter : IMoodAnalyzer
{
    private readonly ExternalMoodApiClient _external;

    public MoodAnalyticsAdapter(ExternalMoodApiClient? client = null)
    {
        _external = client ?? new ExternalMoodApiClient();
    }

    public async Task<MoodResult> AnalyzeAsync(string text)
    {
        var dto = await _external.AnalyzeAsync(text);

        return new MoodResult
        {
            Label          = MapLabel(dto.emotion_code),
            Confidence     = dto.confidence_pct / 100.0,
            Summary        = dto.raw_summary,
            DetectedThemes = dto.keywords.ToList()
        };
    }

    private static MoodLabel MapLabel(string code) => code switch
    {
        "happy"    => MoodLabel.Happy,
        "sad"      => MoodLabel.Sad,
        "stressed" => MoodLabel.Stressed,
        "fatigued" => MoodLabel.Neutral,
        "anxious"  => MoodLabel.Anxious,
        "calm"     => MoodLabel.Calm,
        _          => MoodLabel.Neutral
    };
}
