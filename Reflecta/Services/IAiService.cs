namespace Reflecta.Services;

public interface IAiService
{
    Task<string> GetReflectionAsync(string entryText);
    Task<string> GetChatResponseAsync(string userMessage, IEnumerable<string> history);
}
