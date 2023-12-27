using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace VBAudioRouter.Utils;

internal static class Extensions
{
    /// <summary>
    /// https://stackoverflow.com/a/14353572/15213858
    /// </summary>
    /// <returns></returns>
    public static double Map(this double value, double fromSource, double toSource, double fromTarget, double toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }

    public static T? FindNameRecursive<T>(this FrameworkElement ele, string name) where T : FrameworkElement
    {
        for (var child_index = 0; child_index <= VisualTreeHelper.GetChildrenCount(ele) - 1; child_index++)
        {
            FrameworkElement child = (FrameworkElement)VisualTreeHelper.GetChild(ele, child_index);

            var search = (T?)child.FindName(name);
            if (search != null)
                return search;

            var recursion = child.FindNameRecursive<T>(name);
            if (recursion != null)
                return recursion;
        }
        return null;
    }

    public static FrameworkElement FindNameRecursive(this FrameworkElement ele, string name)
        => ele.FindNameRecursive(name);

    /// <summary>
    /// https://stackoverflow.com/a/24120993/15213858
    /// </summary>
    /// <param name="this"></param>
    public static void BringToFront(this FrameworkElement @this)
    {
        if (@this.Parent is not Panel parent)
            return;

        int currentIndex = Canvas.GetZIndex(@this);
        int maxZ = 0;
        foreach (var child in parent.Children)
        {
            if (child == @this)
                continue;

            var zIndex = Canvas.GetZIndex(child);
            maxZ = Math.Max(maxZ, zIndex);

            if (zIndex >= currentIndex)
                Canvas.SetZIndex(child, zIndex - 1);
        }

        Canvas.SetZIndex(@this, maxZ);
    }
}

internal static class ColorTranslator
{
    /// <summary>
    /// http://joeljoseph.net/converting-hex-to-color-in-universal-windows-platform-uwp/
    /// </summary>
    public static Color FromHex(string hex)
    {
        hex = hex.Replace("#", string.Empty);
        byte a = 255;
        byte r = System.Convert.ToByte(Convert.ToUInt32(hex.Substring(0, 2), 16));
        byte g = System.Convert.ToByte(Convert.ToUInt32(hex.Substring(2, 2), 16));
        byte b = System.Convert.ToByte(Convert.ToUInt32(hex.Substring(4, 2), 16));
        return Color.FromArgb(a, r, g, b);
    }
}