namespace Reflecta.Views.Controls;

/// <summary>
/// Reusable card container — dark surface, rounded corners, translucent border.
/// Use in XAML exactly like a Border; child view becomes the card body.
/// </summary>
public partial class CardView : Border
{
    public CardView() => InitializeComponent();
}
