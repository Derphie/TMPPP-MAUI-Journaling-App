using Reflecta.ViewModels;

namespace Reflecta.Views;

public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _vm;

    // Parameterless ctor for Shell DataTemplate
    public ProfilePage() : this(IPlatformApplication.Current!.Services.GetRequiredService<ProfileViewModel>()) { }

    public ProfilePage(ProfileViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitializeAsync();
    }
}
