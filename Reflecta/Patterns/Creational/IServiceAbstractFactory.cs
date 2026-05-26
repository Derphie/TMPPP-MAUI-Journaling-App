using Reflecta.Services;

namespace Reflecta.Patterns.Creational;

public interface IServiceAbstractFactory
{
    INotificationService CreateNotificationService();
    IShareService        CreateShareService();
}

public class AndroidServiceFactory : IServiceAbstractFactory
{
    public INotificationService CreateNotificationService() =>
        new AndroidNotificationService();

    public IShareService CreateShareService() =>
        new AndroidShareService();
}

public class IosServiceFactory : IServiceAbstractFactory
{
    public INotificationService CreateNotificationService() =>
        new IosNotificationService();

    public IShareService CreateShareService() =>
        new IosShareService();
}

public static class ServiceAbstractFactoryResolver
{
    public static IServiceAbstractFactory Resolve() =>
        DeviceInfo.Platform == DevicePlatform.iOS
            ? new IosServiceFactory()
            : new AndroidServiceFactory();
}
