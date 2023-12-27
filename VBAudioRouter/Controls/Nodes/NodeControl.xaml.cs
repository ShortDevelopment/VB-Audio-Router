using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using VBAudioRouter.GraphControl;
using VBAudioRouter.Utils;
using Windows.Foundation;
using Windows.UI.Core;

namespace VBAudioRouter.Controls.Nodes;

[ObservableObject]
[ContentProperty(Name = "NodeContent")]
public sealed partial class NodeControl : UserControl, INodeControl
{
    [ObservableProperty]
    UIElement? _nodeContent;

    [ObservableProperty]
    public string? _title;

    [ObservableProperty]
    public Brush? _titleBrush;

    public NodeControl()
    {
        InitializeComponent();

        DataContext = this;
        // TitleBrush = new SolidColorBrush(ColorTranslator.FromHex("#9E343E"));
    }

    private void UserControl_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        positionTransform.TranslateX += e.Delta.Translation.X;
        positionTransform.TranslateY += e.Delta.Translation.Y;
        if (NodeContent != null)
            NodeContent.InvalidateArrange();
    }

    private void Grid_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        this.BringToFront();
        this.Focus(FocusState.Keyboard);
        e.Handled = true;
    }

    private void Grid_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
    {
        this.BringToFront();
    }

    public Point NodePosition
    {
        get => new(positionTransform.TranslateX, positionTransform.TranslateY);
        set
        {
            positionTransform.TranslateX = value.X;
            positionTransform.TranslateY = value.Y;
        }
    }
}
