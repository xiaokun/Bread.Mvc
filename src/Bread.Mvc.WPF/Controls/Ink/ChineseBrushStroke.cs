using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace Bread.Mvc.WPF;

internal class ChineseBrushStroke : Stroke
{

    private ImageSource? imageSource = null;
    private readonly double width = 16;


    public ChineseBrushStroke(StylusPointCollection stylusPointCollection, Color color) : base(stylusPointCollection)
    {
        if ((bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue)
            return;
        var dv = new DrawingVisual();
        var size = 90;

        var path = AppDomain.CurrentDomain.BaseDirectory + "Skins\\gizmo\\pen.png";
        if (System.IO.File.Exists(path)) {
            using (var conext = dv.RenderOpen()) {
                conext.PushOpacityMask(new ImageBrush(new BitmapImage(new Uri(path, UriKind.Absolute))));
                conext.DrawRectangle(new SolidColorBrush(color), null, new Rect(0, 0, size, size));
                conext.Close();
            }
        }

        var rtb = new RenderTargetBitmap(size, size, 96d, 96d, PixelFormats.Pbgra32);
        rtb.Render(dv);
        imageSource = BitmapFrame.Create(rtb);

        //Freezable 类提供特殊功能，以便在使用修改或复制开销很大的对象时帮助提高应用程序性能
        //WPF中的Frozen（冻结）与线程及其他相关问题
        imageSource.Freeze();
    }

    //卡顿就是该函数造成，每写完一笔就会调用，当笔画过长，后果可想而知~
    protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
    {
        if (StylusPoints == null) return;
        if (this.StylusPoints.Count < 1) return;

        var p1 = new Point(double.NegativeInfinity, double.NegativeInfinity);
        var w1 = this.width + 20;


        for (int i = 0; i < StylusPoints.Count; i++) {
            var p2 = (Point)this.StylusPoints[i];

            var vector = p1 - p2;

            var dx = (p2.X - p1.X) / vector.Length;
            var dy = (p2.Y - p1.Y) / vector.Length;

            var w2 = this.width;
            if (w1 - vector.Length > this.width)
                w2 = w1 - vector.Length;

            for (int j = 0; j < vector.Length; j++) {
                var x = p2.X;
                var y = p2.Y;

                if (!double.IsInfinity(p1.X) && !double.IsInfinity(p1.Y)) {
                    x = p1.X + dx;
                    y = p1.Y + dy;
                }

                drawingContext.DrawImage(imageSource, new Rect(x - w2 / 2.0, y - w2 / 2.0, w2, w2));

                p1 = new Point(x, y);

                if (double.IsInfinity(vector.Length))
                    break;
            }
        }
    }
}
