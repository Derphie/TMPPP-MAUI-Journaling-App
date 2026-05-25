namespace Reflecta.Services;

public interface INotificationService
{
    Task ScheduleAsync(string title, string body, DateTime at);
    Task CancelAllAsync();
}

// ── Concrete implementations (created via Abstract Factory) ────────────────

public class AndroidNotificationService : INotificationService
{
    public Task ScheduleAsync(string title, string body, DateTime at)
    {
        // In production: use Plugin.LocalNotification or AlarmManager.
        // For the demo, we log intent and return.
        System.Diagnostics.Debug.WriteLine(
            $"[Android] Scheduling notification '{title}' at {at:HH:mm}");
        return Task.CompletedTask;
    }

    public Task CancelAllAsync()
    {
        System.Diagnostics.Debug.WriteLine("[Android] Cancelling all notifications.");
        return Task.CompletedTask;
    }
}

public class IosNotificationService : INotificationService
{
    public Task ScheduleAsync(string title, string body, DateTime at)
    {
        System.Diagnostics.Debug.WriteLine(
            $"[iOS] Scheduling notification '{title}' at {at:HH:mm}");
        return Task.CompletedTask;
    }

    public Task CancelAllAsync() => Task.CompletedTask;
}
