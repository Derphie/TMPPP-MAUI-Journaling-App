using Reflecta.ViewModels;

namespace Reflecta.Views;

public partial class ChatPage : ContentPage
{
    private readonly ChatViewModel _vm;

    // Parameterless ctor for Shell DataTemplate (resolves ViewModel via service locator)
    public ChatPage() : this(IPlatformApplication.Current!.Services.GetRequiredService<ChatViewModel>()) { }

    public ChatPage(ChatViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitializeAsync();
        ScrollToBottom();
        _vm.Messages.CollectionChanged += (_, _) => ScrollToBottom();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _vm.Messages.CollectionChanged -= (_, _) => ScrollToBottom();
    }

    private void ScrollToBottom()
    {
        if (_vm.Messages.Count > 0)
            MessagesCollection.ScrollTo(_vm.Messages[^1], animate: false);
    }
}
