using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Reflecta.Models;
using Reflecta.Patterns.Structural;
using Reflecta.Repositories;

namespace Reflecta.ViewModels;

public partial class ChatViewModel : BaseViewModel
{
    private readonly ReflectaFacade  _facade;
    private readonly IChatRepository _chat;

    public ObservableCollection<ChatMessage> Messages { get; } = new();

    [ObservableProperty] private string _inputText = string.Empty;

    public string[] SuggestedReplies { get; } =
    [
        "I'm feeling stressed",
        "Give me advice",
        "I need motivation",
        "How can I sleep better?",
        "I'm feeling anxious",
    ];

    public ChatViewModel(ReflectaFacade facade, IChatRepository chat)
    {
        _facade = facade;
        _chat   = chat;
        Title   = "Chat";
    }

    public async Task InitializeAsync()
    {
        var history = await _chat.GetAllAsync();
        Messages.Clear();
        foreach (var m in history) Messages.Add(m);
    }

    [RelayCommand]
    private async Task SendMessageAsync()
    {
        var text = InputText.Trim();
        if (string.IsNullOrEmpty(text)) return;

        InputText = string.Empty;

        var userMsg = new ChatMessage { Text = text, IsUser = true, Timestamp = DateTime.Now };
        Messages.Add(userMsg);
        await _chat.SaveMessageAsync(userMsg);

        await RunSafeAsync(async () =>
        {
            var recentHistory = Messages.TakeLast(10).Select(m => m.Text);
            var aiText = await _facade.GetAiChatResponseAsync(text, recentHistory);

            var aiMsg = new ChatMessage { Text = aiText, IsUser = false, Timestamp = DateTime.Now };
            Messages.Add(aiMsg);
            await _chat.SaveMessageAsync(aiMsg);
        });
    }

    [RelayCommand]
    private Task UseSuggestedReply(string reply)
    {
        InputText = reply;
        return SendMessageAsync();
    }

    [RelayCommand]
    private async Task ClearChatAsync()
    {
        await _chat.ClearAsync();
        Messages.Clear();
    }
}
