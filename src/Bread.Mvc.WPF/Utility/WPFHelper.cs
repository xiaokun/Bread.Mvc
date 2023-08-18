using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Bread.Utility.IO;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using FontFamily = System.Windows.Media.FontFamily;
using FontStyle = System.Windows.FontStyle;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Bread.Mvc.WPF;

public static class WPFHelper
{
    #region visual tree

    public static DependencyObject? FindChild(DependencyObject o, Type type)
    {
        if (o.GetType() == type) {
            return o;
        }

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++) {
            var child = VisualTreeHelper.GetChild(o, i);

            var result = FindChild(child, type);
            if (result == null) {
                continue;
            }
            else {
                return result;
            }
        }
        return null;
    }

    public static T? GetParentOfType<T>(this DependencyObject element) where T : DependencyObject
    {
        if (element == null) return null;
        if (element is not FrameworkElement fe) return null;

        var parent = VisualTreeHelper.GetParent(element);
        if (parent == null && fe.Parent is DependencyObject)
            parent = fe.Parent;
        if (parent == null) return null;

        var type = typeof(T);
        if (parent.GetType() == type || parent.GetType().IsSubclassOf(type))
            return parent as T;

        return GetParentOfType<T>(parent);
    }

    /// <summary>
    /// 利用VisualTreeHelper寻找指定依赖对象的父级对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    private static T? FindVisualParent<T>(this DependencyObject obj) where T : DependencyObject
    {
        try {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            if (parent == null) return null;

            if (parent is T target) {
                return target;
            }

            return FindVisualParent<T>(parent);
        }
        catch (Exception ex) {
            Log.Exception(ex);
            return null;
        }
    }

    public static T? GetChildOfType<T>(this DependencyObject source) where T : DependencyObject
    {
        if (source == null) return null;

        var childs = GetChildObjects(source);
        foreach (DependencyObject child in childs) {
            if (child == null) continue;

            if (child is T) {
                return (T)child;
            }

            var dest = GetChildOfType<T>(child);
            if (dest != null && dest is T) {
                return (T)dest;
            }
        }

        return null;
    }

    private static IEnumerable<DependencyObject> GetChildObjects(this DependencyObject parent)
    {
        if (parent == null) yield break;

        if (parent is ContentElement) {
            //use the logical tree for content / framework elements
            foreach (object obj in LogicalTreeHelper.GetChildren(parent)) {
                var child = obj as DependencyObject;
                if (child != null) yield return child;
            }
        }
        else {
            //use the visual tree per default
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++) {
                yield return VisualTreeHelper.GetChild(parent, i);
            }
        }
    }



    #endregion

    #region Color
    public static Color ToColor(this string colorName)
    {
        if (colorName.StartsWith("#"))
            colorName = colorName.Replace("#", string.Empty);
        int v = int.Parse(colorName, System.Globalization.NumberStyles.HexNumber);
        return ToColor(v);
    }

    public static Color ToColor(this int rgba)
    {
        return new Color() {
            A = Convert.ToByte((rgba >> 24) & 255),
            B = Convert.ToByte((rgba >> 16) & 255),
            G = Convert.ToByte((rgba >> 8) & 255),
            R = Convert.ToByte((rgba >> 0) & 255)
        };
    }

    public static Color ToColor(this uint rgba)
    {
        return new Color() {
            A = Convert.ToByte((rgba >> 24) & 255),
            B = Convert.ToByte((rgba >> 16) & 255),
            G = Convert.ToByte((rgba >> 8) & 255),
            R = Convert.ToByte((rgba >> 0) & 255)
        };
    }

    public static int ToInt32(this Color color)
    {
        uint value = 0;
        value |= ((uint)color.A) << 24;
        value |= ((uint)color.R) << 16;
        value |= ((uint)color.G) << 8;
        value |= ((uint)color.B);
        return unchecked((int)value);
    }


    public static uint ToUInt32(this Color color)
    {
        uint value = 0;
        value |= ((uint)color.A) << 24;
        value |= ((uint)color.R) << 16;
        value |= ((uint)color.G) << 8;
        value |= ((uint)color.B);
        return value;
    }


    public static Color GetRelativeColor(this GradientStopCollection gsc, double offset)
    {
        GradientStop before = gsc.Where(w => w.Offset == gsc.Min(m => m.Offset)).First();
        GradientStop after = gsc.Where(w => w.Offset == gsc.Max(m => m.Offset)).First();

        foreach (var gs in gsc) {
            if (gs.Offset <= offset && gs.Offset > before.Offset) {
                before = gs;
            }
            else if (gs.Offset >= offset && gs.Offset < after.Offset) {
                after = gs;
            }
        }

        if (before == after) return before.Color;

        var color = new Color();

        color.ScA = (float)((offset - before.Offset) * (after.Color.ScA - before.Color.ScA) / (after.Offset - before.Offset) + before.Color.ScA);
        color.ScR = (float)((offset - before.Offset) * (after.Color.ScR - before.Color.ScR) / (after.Offset - before.Offset) + before.Color.ScR);
        color.ScG = (float)((offset - before.Offset) * (after.Color.ScG - before.Color.ScG) / (after.Offset - before.Offset) + before.Color.ScG);
        color.ScB = (float)((offset - before.Offset) * (after.Color.ScB - before.Color.ScB) / (after.Offset - before.Offset) + before.Color.ScB);

        return color;
    }

    #endregion

    #region Rect Size Point

    public static System.Drawing.RectangleF ToRectangleF(this Rect rect)
    {
        return new System.Drawing.RectangleF((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
    }

    public static System.Drawing.Rectangle ToRectangle(this Rect rect)
    {
        return new System.Drawing.Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
    }

    public static System.Drawing.SizeF ToSizeF(this Rect rect)
    {
        return new System.Drawing.SizeF((float)rect.Width, (float)rect.Height);
    }

    public static System.Drawing.Size ToSize(this Rect size)
    {
        return new System.Drawing.Size((int)size.Width, (int)size.Height);
    }

    public static System.Drawing.RectangleF ToRectangleF(this Size size)
    {
        return new System.Drawing.RectangleF(0, 0, (float)size.Width, (float)size.Height);
    }

    public static System.Drawing.Rectangle ToRectangle(this Size size)
    {
        return new System.Drawing.Rectangle(0, 0, (int)size.Width, (int)size.Height);
    }

    public static System.Drawing.SizeF ToSizeF(this Size size)
    {
        return new System.Drawing.SizeF((float)size.Width, (float)size.Height);
    }

    public static System.Drawing.Size ToSize(this Size size)
    {
        return new System.Drawing.Size((int)size.Width, (int)size.Height);
    }

    public static Rect ToRect(this System.Drawing.RectangleF rect)
    {
        return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
    }

    public static Rect ToRect(this System.Drawing.Rectangle rect)
    {
        return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
    }

    public static Size ToSize(this System.Drawing.RectangleF rect)
    {
        return new Size(rect.Width, rect.Height);
    }

    public static Size ToSize(this System.Drawing.Rectangle rect)
    {
        return new Size(rect.Width, rect.Height);
    }

    public static Size ToSize(this System.Drawing.SizeF size)
    {
        return new Size(size.Width, size.Height);
    }

    public static Size ToSize(this System.Drawing.Size size)
    {
        return new Size(size.Width, size.Height);
    }

    public static Point ToPoint(this System.Drawing.Point p)
    {
        return new Point(p.X, p.Y);
    }

    public static Point ToPoint(this System.Drawing.PointF p)
    {
        return new Point(p.X, p.Y);
    }

    public static System.Drawing.Point ToPoint(this Point p)
    {
        return new System.Drawing.Point((int)p.X, (int)p.Y);
    }

    public static System.Drawing.PointF ToPointF(this Point p)
    {
        return new System.Drawing.PointF((float)p.X, (float)p.Y);
    }

    public static System.Drawing.Rectangle GetRectangle(this Rect rect, double width, double height)
    {
        if (rect.Width > 1 || rect.Height > 1) {
            return rect.ToRectangle();
        }
        return new System.Drawing.Rectangle((int)(rect.X * width), (int)(rect.Y * height), (int)(rect.Width * width), (int)(rect.Height * height));
    }

    public static System.Drawing.RectangleF GetRectangleF(this Rect rect, double width, double height)
    {
        if (rect.Width > 1 || rect.Height > 1) {
            return rect.ToRectangleF();
        }
        return new System.Drawing.RectangleF((float)(rect.X * width), (float)(rect.Y * height), (float)(rect.Width * width), (float)(rect.Height * height));
    }

    public static System.Drawing.Rectangle GetRectangle(this System.Drawing.RectangleF rect, double width, double height)
    {
        if (rect.Width > 1 || rect.Height > 1) {
            return new System.Drawing.Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }
        return new System.Drawing.Rectangle((int)(rect.X * width), (int)(rect.Y * height), (int)(rect.Width * width), (int)(rect.Height * height));
    }

    public static System.Drawing.RectangleF GetRectangleF(this System.Drawing.RectangleF rect, double width, double height)
    {
        if (rect.Width > 1 || rect.Height > 1) {
            return rect;
        }
        return new System.Drawing.RectangleF((float)(rect.X * width), (float)(rect.Y * height), (float)(rect.Width * width), (float)(rect.Height * height));
    }

    public static Point[]? ToPoints(this System.Drawing.PointF[] points)
    {
        if (points == null) return null;
        int len = points.Length;
        if (len <= 0) return null;

        var ps = new Point[len];
        for (int i = 0; i < len; i++) {
            ps[i] = points[i].ToPoint();
        }
        return ps;
    }

    public static System.Drawing.PointF[]? ToPoints(this Point[] points)
    {
        if (points == null) return null;
        int len = points.Length;
        if (len <= 0) return null;

        var ps = new System.Drawing.PointF[len];
        for (int i = 0; i < len; i++) {
            ps[i] = points[i].ToPointF();
        }
        return ps;
    }
    #endregion

    #region DPI

    public struct Dpi
    {
        public double X { get; set; }

        public double Y { get; set; }

        public Dpi(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public static Dpi GetDpiFromVisual(Visual visual)
    {
        var source = PresentationSource.FromVisual(visual);

        var dpiX = 1.0;
        var dpiY = 1.0;

        if (source?.CompositionTarget != null) {
            dpiX = source.CompositionTarget.TransformToDevice.M11;
            dpiY = source.CompositionTarget.TransformToDevice.M22;
        }

        return new Dpi(dpiX, dpiY);
    }


    #endregion

    /// <summary>
    /// 窗口抖动效果
    /// </summary>
    /// <param name="window"></param>
    public static void Shake(this Window window)
    {
        int i, j, k; //定义三个变量

        for (i = 1; i <= 3; i++) //循环次数
        {
            for (j = 1; j <= 5; j++) {
                window.Top += 1;
                window.Left += 1;
                System.Threading.Thread.Sleep(5); //当前线程指定挂起的时间
            }

            for (k = 1; k <= 5; k++) {
                window.Top -= 1;
                window.Left -= 1;
                System.Threading.Thread.Sleep(5);
            }
        }
    }


    public static Typeface CreateTypeface(this FrameworkElement fe)
    {
        return new Typeface((FontFamily)fe.GetValue(TextBlock.FontFamilyProperty),
                            (FontStyle)fe.GetValue(TextBlock.FontStyleProperty),
                            (FontWeight)fe.GetValue(TextBlock.FontWeightProperty),
                            (FontStretch)fe.GetValue(TextBlock.FontStretchProperty));
    }

    public static ImageSource CreateImageSource(string exeAssemblyName, string path)
    {
        var newImage = new BitmapImage();
        newImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
        newImage.CacheOption = BitmapCacheOption.None;
        var urisource = new Uri($"pack://application:,,,/exeAssemblyName;component{path}", UriKind.Absolute);
        newImage.BeginInit();
        newImage.UriSource = urisource;
        newImage.EndInit();

        return newImage;
    }


    /// <summary>
    /// Draw inner rect snaps to the device pixel
    /// </summary>
    public static void DrawSnappedRect(this DrawingContext drawingContext,
        Brush brush, Pen pen, Rect rect)
    {
        double halfPenWidth = pen?.Thickness / 2 ?? 0;

        // Create a guidelines set
        GuidelineSet guidelines = new GuidelineSet();
        guidelines.GuidelinesX.Add(rect.Left + halfPenWidth);
        guidelines.GuidelinesX.Add(rect.Right - 1 - halfPenWidth);
        guidelines.GuidelinesY.Add(rect.Top + halfPenWidth);
        guidelines.GuidelinesY.Add(rect.Bottom - 1 - halfPenWidth);

        drawingContext.PushGuidelineSet(guidelines);
        rect.Width -= 1;
        rect.Height -= 1;
        drawingContext.DrawRectangle(brush, pen, rect);
        drawingContext.Pop();
    }
}
