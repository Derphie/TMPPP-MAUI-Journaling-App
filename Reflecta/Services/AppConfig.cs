namespace Reflecta.Services;

public static class AppConfig
{
    /// <summary>Base URL of the ngrok-exposed AI endpoint. Swap this when the tunnel restarts.</summary>
    public static string AiBaseUrl = "https://sandfish-celibacy-press.ngrok-free.dev";

    /// <summary>True → route AI calls through HttpAiService (ngrok). False → MockAiService only.</summary>
    public static bool UseRemoteAi = true;
}
