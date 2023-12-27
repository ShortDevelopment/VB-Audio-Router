using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;

namespace VBAudioRouter.Controls.Nodes;
internal interface INodeControl
{
    Brush TitleBrush
    {
        get; set;
    }
    string Title
    {
        get; set;
    }

    UIElement NodeContent
    {
        get; set;
    }
    DependencyObject Parent
    {
        get;
    }
    Point NodePosition
    {
        get; set;
    }
}