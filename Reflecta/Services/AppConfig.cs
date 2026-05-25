namespace Reflecta.Services;

public static class AppConfig
{
    /// <summary>Base URL of the ngrok-exposed AI endpoint. Swap this constant when the tunnel restarts.</summary>
    public const string AiBaseUrl = "https://your-tunnel.ngrok-free.app";

    /// <summary>
    /// True → route AI calls through HttpAiService (ngrok).
    /// False → use MockAiService only (safe offline default).
    /// Change this to true when your ngrok tunnel is running.
    /// </summary>
    public static readonly bool UseRemoteAi = false;
}
