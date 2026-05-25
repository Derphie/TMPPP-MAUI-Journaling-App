namespace Reflecta.Views.Controls;

public partial class MessageBubble : ContentView
{
    public static readonly BindableProperty MessageTextProperty =
        BindableProperty.Create(nameof(MessageText), typeof(string), typeof(MessageBubble), string.Empty,
            propertyChanged: (b, _, _) => (b as MessageBubble)?.Update());

    public static readonly BindableProperty IsUserMessageProperty =
        BindableProperty.Create(nameof(IsUserMessage), typeof(bool), typeof(MessageBubble), false,
            propertyChanged: (b, _, _) => (b as MessageBubble)?.Update());

    public static readonly BindableProperty TimestampTextProperty =
        BindableProperty.Create(nameof(TimestampText), typeof(string), typeof(MessageBubble), string.Empty,
            propertyChanged: (b, _, _) => (b as MessageBubble)?.Update());

    public string MessageText
    {
        get => (string)GetValue(MessageTextProperty);
        set => SetValue(MessageTextProperty, value);
    }

    public bool IsUserMessage
    {
        get => (bool)GetValue(IsUserMessageProperty);
        set => SetValue(IsUserMessageProperty, value);
    }

    public string TimestampText
    {
        get => (string)GetValue(TimestampTextProperty);
        set => SetValue(TimestampTextProperty, value);
    }

    public MessageBubble() => InitializeComponent();

    private void Update()
    {
        if (IsUserMessage)
        {
            UserBubbleContainer.IsVisible = true;
            AiBubbleContainer.IsVisible   = false;
            UserText.Text       = MessageText;
            UserTimestamp.Text  = TimestampText;
        }
        else
        {
            AiBubbleContainer.IsVisible   = true;
            UserBubbleContainer.IsVisible = false;
            AiText.Text       = MessageText;
            AiTimestamp.Text  = TimestampText;
        }
    }
}
