using Reflecta.Services;
using Reflecta.Views;

namespace Reflecta;

public partial class App : Application
{
    private readonly SeedDataService _seeder;
    private readonly OnboardingPage  _onboarding;

    public App(SeedDataService seeder, OnboardingPage onboarding)
    {
        InitializeComponent();
        _seeder     = seeder;
        _onboarding = onboarding;
    }

    protected override async void OnStart()
    {
        base.OnStart();
        await _seeder.SeedAsync();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(_onboarding);
    }

    /// <summary>Called by OnboardingViewModel.GetStartedCommand to switch to the main shell.</summary>
    public static void NavigateToShell()
    {
        if (Current?.Windows.Count > 0)
            Current.Windows[0].Page = new AppShell();
    }
}
