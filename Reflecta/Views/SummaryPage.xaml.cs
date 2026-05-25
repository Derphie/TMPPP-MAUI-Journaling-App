using Reflecta.ViewModels;

namespace Reflecta.Views;

public partial class SummaryPage : ContentPage
{
    private readonly SummaryViewModel _vm;

    // Parameterless ctor for Shell DataTemplate
    public SummaryPage() : this(IPlatformApplication.Current!.Services.GetRequiredService<SummaryViewModel>()) { }

    public SummaryPage(SummaryViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitializeAsync();
    }

    private async void OnPeriodTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not string param || !int.TryParse(param, out int idx)) return;

        _vm.PeriodIndex = idx;

        LblDaily.TextColor   = idx == 0 ? Color.FromArgb("#A855F7") : Color.FromArgb("#8B8B9A");
        LblWeekly.TextColor  = idx == 1 ? Color.FromArgb("#A855F7") : Color.FromArgb("#8B8B9A");
        LblMonthly.TextColor = idx == 2 ? Color.FromArgb("#A855F7") : Color.FromArgb("#8B8B9A");

        BtnDaily.BackgroundColor   = idx == 0 ? Color.FromArgb("#33A855F7") : Colors.Transparent;
        BtnWeekly.BackgroundColor  = idx == 1 ? Color.FromArgb("#33A855F7") : Colors.Transparent;
        BtnMonthly.BackgroundColor = idx == 2 ? Color.FromArgb("#33A855F7") : Colors.Transparent;

        await _vm.LoadCommand.ExecuteAsync(null);
    }
}
