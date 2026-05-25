namespace Reflecta.Services;

public static class AppConfig
{
    /// <summary>Base URL of the deployed AI endpoint on Render.</summary>
    public static string AiBaseUrl = "https://tmppp-maui-journaling-app.onrender.com";

    /// <summary>True → route AI calls through HttpAiService. False → MockAiService only.</summary>
    public static bool UseRemoteAi = true;
}