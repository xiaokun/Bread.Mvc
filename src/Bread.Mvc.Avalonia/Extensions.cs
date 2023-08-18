using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using SkiaSharp;

namespace Bread.Mvc.Avalonia;

public static class Extensions
{
    public static void SetCheck(this StyledElement se, bool ischecked)
    {
        if (ischecked) {
            if (se.Classes.Contains("unchecked")) {
                se.Classes.Remove("unchecked");
            }
            if (se.Classes.Contains("checked") == false) {
                se.Classes.Add("checked");
            }
        }
        else {
            if (se.Classes.Contains("checked")) {
                se.Classes.Remove("checked");
            }
            if (se.Classes.Contains("unchecked") == false) {
                se.Classes.Add("unchecked");
            }
        }
    }

    public static Color FindColor(this Control ctr, string key)
    {
        if (Application.Current!.TryFindResource(key, out object? value)) {
            if (value is not null && value is Color color) {
                return color;
            }
        }
        throw new KeyNotFoundException(key);
    }

    public static SolidColorBrush FindBrush(this Control ctr, string key)
    {

        if (Application.Current!.TryFindResource(key, out object? value)) {
            if (value is not null && value is SolidColorBrush brush) {
                return brush;
            }
        }
        throw new KeyNotFoundException(key);
    }

    public static T FindResource<T>(this Control ctr, string rawUri) where T : class
    {
        if (rawUri.StartsWith("avares://") == false) {
            throw new ArgumentException("rawUri must start with avares://");
        }
       
        var asset = AssetLoader.Open(new Uri(rawUri));
        if (asset == null) throw new EntryPointNotFoundException(rawUri);
        var obj = Activator.CreateInstance(typeof(T), asset);
        if (obj == null) throw new Exception($"Create instance({nameof(T)}) is failed.");
        var t = obj as T;
        if (t == null) throw new Exception($"instance to {nameof(T)} failed");
        return t;
    }

    private static bool PenIsVisible(IPen? pen)
    {
        return pen?.Brush != null && pen.Thickness > 0;
    }

    public static void DrawRectangle(this DrawingContext dc, IBrush? brush, IPen? pen, Rect rect,
        double radiusTopLeft, double radiusTopRight, double radiusBottomRight, double radiusBottomLeft,
       BoxShadows boxShadows = default)
    {
        if (brush == null && !PenIsVisible(pen)) {
            return;
        }

        var max = Math.Max(rect.Width, rect.Height) / 2;
        radiusTopLeft = Math.Min(radiusTopLeft, max);
        radiusTopRight = Math.Min(radiusTopRight, max);
        radiusBottomRight = Math.Min(radiusBottomRight, max);
        radiusBottomLeft = Math.Min(radiusBottomLeft, max);

        //if (!MathUtilities.IsZero(radiusTopLeft)) {
        //    radiusTopLeft = Math.Min(radiusTopLeft, rect.Width / 2);
        //}

        //if (!MathUtilities.IsZero(radiusTopLeft)) {
        //    radiusTopLeft = Math.Min(radiusTopLeft, rect.Height / 2);
        //}

        //if (!MathUtilities.IsZero(radiusTopRight)) {
        //    radiusTopRight = Math.Min(radiusTopRight, rect.Width / 2);
        //}

        //if (!MathUtilities.IsZero(radiusTopRight)) {
        //    radiusTopRight = Math.Min(radiusTopRight, rect.Height / 2);
        //}

        //if (!MathUtilities.IsZero(radiusBottomRight)) {
        //    radiusBottomRight = Math.Min(radiusBottomRight, rect.Width / 2);
        //}

        //if (!MathUtilities.IsZero(radiusBottomRight)) {
        //    radiusBottomRight = Math.Min(radiusBottomRight, rect.Height / 2);
        //}

        //if (!MathUtilities.IsZero(radiusBottomLeft)) {
        //    radiusBottomLeft = Math.Min(radiusBottomLeft, rect.Width / 2);
        //}

        //if (!MathUtilities.IsZero(radiusBottomLeft)) {
        //    radiusBottomLeft = Math.Min(radiusBottomLeft, rect.Height / 2);
        //}

        dc.DrawRectangle(brush, pen,
            new RoundedRect(rect, radiusTopLeft, radiusTopRight, radiusBottomRight, radiusBottomLeft),
            boxShadows);
    }

    public static FormattedText CreateFormattedText(this Control control, string content, IBrush brush)
    {
        var typeface = new Typeface(control.GetValue(TextBlock.FontFamilyProperty),
                control.GetValue(TextBlock.FontStyleProperty),
                control.GetValue(TextBlock.FontWeightProperty));
        var size = control.GetValue(TextBlock.FontSizeProperty);
        return new FormattedText(content, Thread.CurrentThread.CurrentCulture, FlowDirection.LeftToRight, typeface, size, brush);
    }

    public static FormattedText CreateFormattedText(this Control control, string content, IBrush brush, double fontsize)
    {
        var typeface = new Typeface(control.GetValue(TextBlock.FontFamilyProperty),
                control.GetValue(TextBlock.FontStyleProperty),
                control.GetValue(TextBlock.FontWeightProperty));
        return new FormattedText(content, Thread.CurrentThread.CurrentCulture, FlowDirection.LeftToRight, typeface, fontsize, brush);
    }

    public static Color ToAvalonia(this SKColor color)
    {
        return new Color(color.Alpha, color.Red, color.Green, color.Blue);
    }

    public static SKColor ToSKColor(this Color color)
    {
        return new SKColor(color.R, color.G, color.B, color.A);
    }

    public static Rect WithXY(this Rect rc, double x, double y)
    {
        return new Rect(x, y, rc.Width, rc.Height);
    }

    public static Rect WithWH(this Rect rc, double w, double h)
    {
        return new Rect(rc.X, rc.Y, w, h);
    }

    public static bool IsInside(this Point p, double x, double y, double w, double h)
    {
        return (p.X >= x) && (p.X <= x + w) && (p.Y >= y) && (p.Y <= y + h);
    }
}
