namespace Reflecta.Services;

public class MockAiService : IAiService
{
    private static readonly (string[] keys, string response)[] _rules =
    [
        (["stress", "stressed", "overwhelmed", "pressure", "deadline"],
         "It sounds like you're carrying a lot right now. Stress can feel very heavy — especially when deadlines stack up. " +
         "Try breaking your workload into smaller pieces and take a short walk between tasks. " +
         "What's the single most important thing you could cross off your list today?"),

        (["tired", "exhausted", "sleep", "fatigue", "sleepy"],
         "Rest is not a luxury — it's when your brain consolidates everything you've learned and felt. " +
         "If you can, aim for 7–8 hours tonight. Even a 20-minute nap can reset your focus. " +
         "Be gentle with yourself — tired days are real days too. 💜"),

        (["happy", "great", "wonderful", "excited", "joy", "love"],
         "That positive energy comes through so clearly! ✨ It's worth pausing to savour moments like this. " +
         "What made today feel so good? Writing it down helps you return to this feeling on harder days."),

        (["sad", "depressed", "lonely", "hopeless", "cry"],
         "I hear you, and I'm glad you're writing it out. Sadness has a way of shrinking everything — " +
         "but it won't last forever. Is there one small thing that usually brings you comfort? " +
         "Reaching out to someone you trust, even just to say hello, can make a real difference."),

        (["school", "study", "exam", "finals", "assignment", "university", "college"],
         "Academic pressure is real and valid. Remember: you've made it through every difficult exam so far. " +
         "Try the Pomodoro technique — 25 minutes of focused study, 5-minute break. " +
         "Your worth is not defined by any grade. 📚"),

        (["anxious", "anxiety", "worry", "nervous", "panic"],
         "Anxiety can make the future feel very threatening. One grounding technique: name 5 things you can " +
         "see, 4 you can touch, 3 you can hear, 2 you can smell, 1 you can taste. " +
         "This brings you back to the present moment. You're safe right now. 🌿"),

        (["workout", "exercise", "gym", "run", "yoga", "sport"],
         "That's amazing — movement is one of the best things you can do for your mental health! " +
         "Exercise releases endorphins, reduces cortisol, and improves sleep quality. " +
         "Keep that momentum going; even a short walk counts. 💪"),

        (["work", "job", "boss", "colleague", "meeting", "office"],
         "Work relationships and responsibilities can be surprisingly draining. " +
         "Setting clear boundaries between work-time and rest-time helps a lot. " +
         "Is there something specific about work that's weighing on you?"),

        (["gratitude", "grateful", "thankful", "appreciate"],
         "Practicing gratitude literally rewires the brain toward positivity over time. " +
         "The fact that you're noticing things to be grateful for is a real strength. " +
         "Keep this habit going — even three small things a day makes a measurable difference. 🌟"),
    ];

    private static readonly string[] _defaultResponses =
    [
        "Thank you for sharing that with me. Journaling like this is a genuine act of self-care. " +
        "How are you feeling right now, in this moment?",

        "What you've written shows real self-awareness. That quality helps you grow through difficult situations. " +
        "Is there anything specific you'd like to explore further?",

        "I'm here to listen and reflect with you. Every entry you write adds to a story of growth. " +
        "What would you like to focus on today?",

        "Sometimes putting thoughts into words is the hardest part — and you did it. " +
        "That's worth acknowledging. What feels most alive for you right now?",
    ];

    public Task<string> GetReflectionAsync(string entryText)
    {
        if (string.IsNullOrWhiteSpace(entryText))
            return Task.FromResult("Start by describing how your day has been, and I'll reflect back with you. 💬");

        var lower = entryText.ToLowerInvariant();

        foreach (var (keys, response) in _rules)
        {
            if (keys.Any(k => lower.Contains(k)))
                return Task.FromResult(response);
        }

        var idx = Math.Abs(entryText.GetHashCode()) % _defaultResponses.Length;
        return Task.FromResult(_defaultResponses[idx]);
    }

    public async Task<string> GetChatResponseAsync(string userMessage, IEnumerable<string> history)
    {
        await Task.Delay(400); 
        return await GetReflectionAsync(userMessage);
    }
}
