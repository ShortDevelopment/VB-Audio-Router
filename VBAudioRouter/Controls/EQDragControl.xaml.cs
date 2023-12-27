using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.VisualBasic;
using Windows.Foundation;

namespace VBAudioRouter.Controls;

public sealed partial class EQDragControl : UserControl
{
    public Canvas Canvas
    {
        get; set;
    }
    public int Index
    {
        get; set;
    }

    public event EventHandler<Point> ValueChanged;

    public void SetPosition(Point p)
    {
        var maxX = Canvas.ActualWidth - ActualWidth;
        var maxY = Canvas.ActualHeight - ActualHeight;
        positionTransform.TranslateX = p.X * maxX;
        positionTransform.TranslateY = (1 - p.Y) * maxY;
    }

    private void Grid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        var maxX = Canvas.ActualWidth - ActualWidth;
        var maxY = Canvas.ActualHeight - ActualHeight;
        positionTransform.TranslateX = Math.Min(Math.Max(positionTransform.TranslateX + e.Delta.Translation.X, 0), maxX);
        positionTransform.TranslateY = Math.Min(Math.Max(positionTransform.TranslateY + e.Delta.Translation.Y, 0), maxY);
        ValueChanged?.Invoke(this, new Point(positionTransform.TranslateX / (double)maxX, 1 - (positionTransform.TranslateY / (double)maxY)));
    }
}
