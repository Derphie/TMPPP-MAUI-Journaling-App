using CommunityToolkit.Mvvm.Input;

namespace Reflecta.ViewModels;

public partial class OnboardingViewModel : BaseViewModel
{
    public OnboardingViewModel()
    {
        Title = "Welcome to Reflecta";
    }

    [RelayCommand]
    private void GetStarted()
    {
        App.NavigateToShell();
    }
}
