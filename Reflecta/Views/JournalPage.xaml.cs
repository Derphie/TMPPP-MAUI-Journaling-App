using Reflecta.ViewModels;

namespace Reflecta.Views;

public partial class JournalPage : ContentPage
{
    private readonly JournalViewModel _vm;

    // Parameterless ctor for Shell DataTemplate
    public JournalPage() : this(IPlatformApplication.Current!.Services.GetRequiredService<JournalViewModel>()) { }

    public JournalPage(JournalViewModel vm)
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
