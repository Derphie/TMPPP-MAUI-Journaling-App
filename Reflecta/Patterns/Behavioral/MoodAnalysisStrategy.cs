using Reflecta.Models;
using Reflecta.Services;

namespace Reflecta.Patterns.Behavioral;

// ── PATTERN 10: Strategy ─────────────────────────────────────────────────
// Defines a family of interchangeable mood-analysis algorithms.
// The Summary page lets the user switch strategy at runtime.
// SimpleAnalysisStrategy uses keyword rules (fast, offline).
// AIWeightedStrategy delegates to IAiService (richer, async).

/// <summary>Strategy interface.</summary>
public interface IMoodAnalysisStrategy
{
    string Name { get; }
    Task<MoodResult> AnalyzeMoodAsync(string text);
}

/// <summary>
/// Concrete Strategy A — fast keyword/rule-based analysis.
/// Works fully offline; suitable as the default strategy.
/// </summary>
public class SimpleAnalysisStrategy : IMoodAnalysisStrategy
{
    public string Name => "Simple";

    private static readonly Dictionary<string[], MoodLabel> _rules = new(new StringArrayComparer())
    {
        [new[] { "stress", "overwhelm", "pressure", "deadline" }] = MoodLabel.Stressed,
        [new[] { "anxious", "anxiety", "worry", "nervous" }]      = MoodLabel.Anxious,
        [new[] { "sad", "depressed", "lonely", "hopeless" }]      = MoodLabel.Sad,
        [new[] { "tired", "exhausted", "fatigue", "sleep" }]      = MoodLabel.Neutral,
        [new[] { "happy", "great", "wonderful", "excited", "love" }] = MoodLabel.Happy,
        [new[] { "calm", "peaceful", "relaxed", "content" }]      = MoodLabel.Calm,
    };

    public Task<MoodResult> AnalyzeMoodAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Task.FromResult(new MoodResult { Label = MoodLabel.Neutral, Confidence = 0.5 });

        var lower  = text.ToLowerInvariant();
        var themes = new List<string>();
        var scores = new Dictionary<MoodLabel, int>();

        foreach (var (keywords, label) in _rules)
        {
            foreach (var kw in keywords)
            {
                if (lower.Contains(kw))
                {
                    themes.Add(kw);
                    scores.TryGetValue(label, out int prev);
                    scores[label] = prev + 1;
                }
            }
        }

        MoodLabel winner = MoodLabel.Neutral;
        int       maxHits = 0;
        foreach (var (label, hits) in scores)
        {
            if (hits > maxHits) { maxHits = hits; winner = label; }
        }

        return Task.FromResult(new MoodResult
        {
            Label          = winner,
            Confidence     = maxHits > 0 ? Math.Min(0.5 + maxHits * 0.1, 0.95) : 0.5,
            Summary        = $"Detected {themes.Count} mood-related cues.",
            DetectedThemes = themes.Distinct().ToList()
        });
    }
}

/// <summary>
/// Concrete Strategy B — richer analysis leveraging the AI service.
/// Falls back to SimpleAnalysisStrategy if AI is unavailable.
/// </summary>
public class AIWeightedStrategy : IMoodAnalysisStrategy
{
    private readonly IAiService              _ai;
    private readonly SimpleAnalysisStrategy  _fallback = new();

    public string Name => "AIWeighted";

    public AIWeightedStrategy(IAiService ai) => _ai = ai;

    public async Task<MoodResult> AnalyzeMoodAsync(string text)
    {
        try
        {
            var aiResponse = await _ai.GetReflectionAsync(text);
            var baseResult = await _fallback.AnalyzeMoodAsync(text + " " + aiResponse);

            baseResult.Confidence = Math.Min(baseResult.Confidence + 0.15, 0.99);
            baseResult.Summary    = "AI-weighted analysis: " + baseResult.Summary;
            return baseResult;
        }
        catch
        {
            return await _fallback.AnalyzeMoodAsync(text);
        }
    }
}

// ── Helper ────────────────────────────────────────────────────────────────

file class StringArrayComparer : IEqualityComparer<string[]>
{
    public bool Equals(string[]? x, string[]? y) =>
        x != null && y != null && x.SequenceEqual(y);

    public int GetHashCode(string[] obj) =>
        obj.Aggregate(0, (acc, s) => acc ^ s.GetHashCode());
}
