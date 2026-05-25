using Reflecta.Services;
using Reflecta.Views;

namespace Reflecta;

public partial class App : Application
{
    private readonly SeedDataService _seeder;

    public App(SeedDataService seeder)
    {
        InitializeComponent();
        _seeder = seeder;
    }

    protected override async void OnStart()
    {
        base.OnStart();
        await _seeder.SeedAsync();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Resolve after InitializeComponent so Application.Resources are ready
        var onboarding = IPlatformApplication.Current!.Services.GetRequiredService<OnboardingPage>();
        return new Window(onboarding);
    }

    /// <summary>Called by OnboardingViewModel.GetStartedCommand to switch to the main shell.</summary>
    public static void NavigateToShell()
    {
        if (Current?.Windows.Count > 0)
            Current.Windows[0].Page = new AppShell();
    }
}
