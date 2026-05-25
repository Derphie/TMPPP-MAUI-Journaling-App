using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Reflecta.Services;

// ── Strategy pattern extension ────────────────────────────────────────────
// HttpAiService is the remote concrete implementation of IAiService.
// It is injected into AIWeightedStrategy (Pattern 10) and the Facade (Pattern 6)
// when AppConfig.UseRemoteAi is true, replacing MockAiService transparently.

/// <summary>
/// Calls the ngrok-exposed AI endpoint; falls back to MockAiService on any failure or timeout
/// so the app never breaks during demos when the tunnel is down.
/// </summary>
public class HttpAiService : IAiService
{
    private readonly HttpClient    _http;
    private readonly MockAiService _fallback;

    public HttpAiService(HttpClient http, MockAiService fallback)
    {
        _http     = http;
        _fallback = fallback;
    }

    public async Task<string> GetReflectionAsync(string entryText)
    {
        try
        {
            var req  = new NgrokAiRequest { Message = entryText };
            var resp = await _http.PostAsJsonAsync("/reflect", req);
            if (!resp.IsSuccessStatusCode)
                return await _fallback.GetReflectionAsync(entryText);

            var body = await resp.Content.ReadFromJsonAsync<NgrokAiResponse>();
            return body?.Reply ?? await _fallback.GetReflectionAsync(entryText);
        }
        catch
        {
            return await _fallback.GetReflectionAsync(entryText);
        }
    }

    public async Task<string> GetChatResponseAsync(string userMessage, IEnumerable<string> history)
    {
        try
        {
            var req  = new NgrokAiRequest { Message = userMessage, History = history.ToList() };
            var resp = await _http.PostAsJsonAsync("/chat", req);
            if (!resp.IsSuccessStatusCode)
                return await _fallback.GetChatResponseAsync(userMessage, history);

            var body = await resp.Content.ReadFromJsonAsync<NgrokAiResponse>();
            return body?.Reply ?? await _fallback.GetChatResponseAsync(userMessage, history);
        }
        catch
        {
            return await _fallback.GetChatResponseAsync(userMessage, history);
        }
    }
}

public sealed record NgrokAiRequest
{
    [JsonPropertyName("message")] public string        Message { get; init; } = string.Empty;
    [JsonPropertyName("history")] public List<string>  History { get; init; } = [];
}

public sealed record NgrokAiResponse
{
    [JsonPropertyName("reply")] public string? Reply { get; init; }
    [JsonPropertyName("mood")]  public string? Mood  { get; init; }
}
