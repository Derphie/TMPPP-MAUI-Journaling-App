namespace Reflecta.Views.Controls;

public partial class TagChip : Border
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(TagChip), string.Empty,
            propertyChanged: (b, _, nv) =>
            {
                if (b is TagChip chip) chip.ChipLabel.Text = nv?.ToString() ?? string.Empty;
            });

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public TagChip() => InitializeComponent();
}
