using Reflecta.Services;

namespace Reflecta.Patterns.Creational;

// ── PATTERN 3: Abstract Factory ───────────────────────────────────────────
// Creates families of platform-specific services (notifications, sharing)
// without coupling the app to a concrete platform.  MauiProgram registers the
// correct concrete factory based on DeviceInfo.Platform so ViewModels never
// reference Android or iOS namespaces directly.

/// <summary>Abstract factory — declares creation methods for a service family.</summary>
public interface IServiceAbstractFactory
{
    INotificationService CreateNotificationService();
    IShareService        CreateShareService();
}

/// <summary>Concrete factory for Android — produces Android implementations.</summary>
public class AndroidServiceFactory : IServiceAbstractFactory
{
    public INotificationService CreateNotificationService() =>
        new AndroidNotificationService();

    public IShareService CreateShareService() =>
        new AndroidShareService();
}

/// <summary>
/// Concrete factory for iOS — stub implementations (iOS build not targeted, but
/// the pattern is complete so the codebase is cross-platform ready).
/// </summary>
public class IosServiceFactory : IServiceAbstractFactory
{
    public INotificationService CreateNotificationService() =>
        new IosNotificationService();

    public IShareService CreateShareService() =>
        new IosShareService();
}

/// <summary>Resolves the correct factory at runtime based on the current platform.</summary>
public static class ServiceAbstractFactoryResolver
{
    public static IServiceAbstractFactory Resolve() =>
        DeviceInfo.Platform == DevicePlatform.iOS
            ? new IosServiceFactory()
            : new AndroidServiceFactory();
}
