using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brush = System.Windows.Media.Brush;
using Point = System.Windows.Point;

namespace Bread.Mvc.WPF;

internal class ChineseBrushRenderer : DynamicRenderer
{
    private ImageSource? imageSource;

    private readonly double width = 16;

    protected override void OnDrawingAttributesReplaced()
    {
        if (DesignerProperties.GetIsInDesignMode(this.Element))
            return;

        base.OnDrawingAttributesReplaced();

        var dv = new DrawingVisual();
        var size = 90;

        var path = AppDomain.CurrentDomain.BaseDirectory + "Skins\\gizmo\\pen.png";
        if (System.IO.File.Exists(path)) {
            using (var conext = dv.RenderOpen()) {
                //[关键]OpacityMask了解下？也许有童鞋想到的办法是，各种颜色的图片来一张？
                conext.PushOpacityMask(new ImageBrush(new BitmapImage(new Uri(path, UriKind.Absolute))));
                //用颜色生成画笔画一个矩形
                conext.DrawRectangle(new SolidColorBrush(this.DrawingAttributes.Color), null, new Rect(0, 0, size, size));
                conext.Close();
            }
        }

        var rtb = new RenderTargetBitmap(size, size, 96d, 96d, PixelFormats.Pbgra32);
        rtb.Render(dv);
        imageSource = BitmapFrame.Create(rtb);
        //[重要]此乃解决卡顿问题的关键！
        imageSource.Freeze();
    }

    protected override void OnDraw(DrawingContext drawingContext, StylusPointCollection stylusPoints, Geometry geometry, Brush fillBrush)
    {
        var p1 = new Point(double.NegativeInfinity, double.NegativeInfinity);
        var p2 = new Point(0, 0);
        var w1 = this.width + 20;

        for (int i = 0; i < stylusPoints.Count; i++) {
            p2 = (Point)stylusPoints[i];

            //两点相减得到一个向量[高中数学知识了解下？]
            var vector = p1 - p2;

            //得到 x、y的变化值
            var dx = (p2.X - p1.X) / vector.Length;
            var dy = (p2.Y - p1.Y) / vector.Length;

            var w2 = this.width;
            if (w1 - vector.Length > this.width)
                w2 = w1 - vector.Length;

            //为啥又来一个for？图像重叠，实现笔画的连续性，感兴趣的童鞋可以把for取消掉看看效果
            for (int j = 0; j < vector.Length; j++) {
                var x = p2.X;
                var y = p2.Y;

                if (!double.IsInfinity(p1.X) && !double.IsInfinity(p1.Y)) {
                    x = p1.X + dx;
                    y = p1.Y + dy;
                }

                //画图，没啥可说的
                drawingContext.DrawImage(imageSource, new Rect(x - w2 / 2.0, y - w2 / 2.0, w2, w2));

                //再把新的坐标赋值给p1，以序后来
                p1 = new Point(x, y);

                if (double.IsInfinity(vector.Length))
                    break;

            }
        }
    }
}
