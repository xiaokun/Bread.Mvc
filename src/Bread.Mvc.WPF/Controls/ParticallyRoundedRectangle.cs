using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Bread.Mvc.WPF;

public class ParticallyRoundedRectangle : Shape
{
    public static readonly DependencyProperty RadiusXProperty;
    public static readonly DependencyProperty RadiusYProperty;

    public static readonly DependencyProperty RoundTopLeftProperty;
    public static readonly DependencyProperty RoundTopRightProperty;
    public static readonly DependencyProperty RoundBottomLeftProperty;
    public static readonly DependencyProperty RoundBottomRightProperty;

    public int RadiusX
    {
        get { return (int)GetValue(RadiusXProperty); }
        set { SetValue(RadiusXProperty, value); }
    }

    public int RadiusY
    {
        get { return (int)GetValue(RadiusYProperty); }
        set { SetValue(RadiusYProperty, value); }
    }

    public bool RoundTopLeft
    {
        get { return (bool)GetValue(RoundTopLeftProperty); }
        set { SetValue(RoundTopLeftProperty, value); }
    }

    public bool RoundTopRight
    {
        get { return (bool)GetValue(RoundTopRightProperty); }
        set { SetValue(RoundTopRightProperty, value); }
    }

    public bool RoundBottomLeft
    {
        get { return (bool)GetValue(RoundBottomLeftProperty); }
        set { SetValue(RoundBottomLeftProperty, value); }
    }

    public bool RoundBottomRight
    {
        get { return (bool)GetValue(RoundBottomRightProperty); }
        set { SetValue(RoundBottomRightProperty, value); }
    }

    static ParticallyRoundedRectangle()
    {
        RadiusXProperty = DependencyProperty.Register
            ("RadiusX", typeof(int), typeof(ParticallyRoundedRectangle));
        RadiusYProperty = DependencyProperty.Register
            ("RadiusY", typeof(int), typeof(ParticallyRoundedRectangle));

        RoundTopLeftProperty = DependencyProperty.Register
            ("RoundTopLeft", typeof(bool), typeof(ParticallyRoundedRectangle));
        RoundTopRightProperty = DependencyProperty.Register
            ("RoundTopRight", typeof(bool), typeof(ParticallyRoundedRectangle));
        RoundBottomLeftProperty = DependencyProperty.Register
            ("RoundBottomLeft", typeof(bool), typeof(ParticallyRoundedRectangle));
        RoundBottomRightProperty = DependencyProperty.Register
            ("RoundBottomRight", typeof(bool), typeof(ParticallyRoundedRectangle));
    }

    public ParticallyRoundedRectangle()
    {
    }

    protected override Geometry DefiningGeometry
    {
        get
        {
            Geometry result = new RectangleGeometry
            (new Rect(0, 0, base.Width, base.Height), RadiusX, RadiusY);
            double halfWidth = base.Width / 2;
            double halfHeight = base.Height / 2;

            if (!RoundTopLeft)
                result = new CombinedGeometry
            (GeometryCombineMode.Union, result, new RectangleGeometry
            (new Rect(0, 0, halfWidth, halfHeight)));
            if (!RoundTopRight)
                result = new CombinedGeometry
            (GeometryCombineMode.Union, result, new RectangleGeometry
            (new Rect(halfWidth, 0, halfWidth, halfHeight)));
            if (!RoundBottomLeft)
                result = new CombinedGeometry
            (GeometryCombineMode.Union, result, new RectangleGeometry
            (new Rect(0, halfHeight, halfWidth, halfHeight)));
            if (!RoundBottomRight)
                result = new CombinedGeometry
            (GeometryCombineMode.Union, result, new RectangleGeometry
            (new Rect(halfWidth, halfHeight, halfWidth, halfHeight)));

            return result;
        }
    }
}
