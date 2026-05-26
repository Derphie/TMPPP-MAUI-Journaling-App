using CommunityToolkit.Mvvm.ComponentModel;

namespace Reflecta.ViewModels;


public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = string.Empty;

    public bool IsNotBusy => !IsBusy;

    protected async Task RunSafeAsync(Func<Task> action, string errorTitle = "Error")
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[{errorTitle}] {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
