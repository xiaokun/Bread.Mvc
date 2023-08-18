using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Bread.Mvc.WPF;

/// <summary>
/// 用于显示音频跳动
/// </summary>
public class AudioIndicator : Canvas
{
    public double Loudness
    {
        set { SetValue(LoudnessProperty, value); }
        get { return (double)GetValue(LoudnessProperty); }
    }
    public static readonly DependencyProperty LoudnessProperty =
        DependencyProperty.Register("Loudness", typeof(double), typeof(AudioIndicator),
            new PropertyMetadata(0.0, OnLoudnessChanged));


    public Orientation Orientation
    {
        set { SetValue(OrientationProperty, value); }
        get { return (Orientation)GetValue(OrientationProperty); }
    }
    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register("Orientation", typeof(Orientation), typeof(AudioIndicator),
            new PropertyMetadata(Orientation.Horizontal));


    public Brush VolumeBrush
    {
        set { SetValue(VolumeBrushProperty, value); }
        get { return (Brush)GetValue(VolumeBrushProperty); }
    }
    public static readonly DependencyProperty VolumeBrushProperty =
        DependencyProperty.Register("VolumeBrush", typeof(Brush), typeof(AudioIndicator),
            new PropertyMetadata(null));

    public AudioIndicator()
    {
    }

    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);

        var bounds = new Rect(new Point(0, 0), RenderSize);
        dc.DrawRectangle(Background, null, bounds);

        if (VolumeBrush == null) return;

        bounds.X += Margin.Left;
        bounds.Y += Margin.Top;
        bounds.Width -= Margin.Left + Margin.Right;
        bounds.Height -= Margin.Top + Margin.Bottom;

        var itemSize = 10;

        if (Orientation == Orientation.Vertical) {
            var factor = Math.Max(0, Math.Min(1.0, Loudness));
            var height = bounds.Height * factor;
            int count = (int)(height / itemSize);
            if (count <= 0) return;

            for (int i = 1; i <= count; i++) {
                var rect = new Rect(bounds.X, bounds.Height - itemSize * i + 3, bounds.Width, itemSize - 3);
                if (VolumeBrush is SolidColorBrush solid) {
                    dc.DrawRoundedRectangle(solid, null, rect, 2, 2);
                }
                else if (VolumeBrush is GradientBrush gradient) {
                    var color = gradient.GradientStops.GetRelativeColor(itemSize * i / bounds.Height);
                    var brush = new SolidColorBrush(color);
                    dc.DrawRoundedRectangle(brush, null, rect, 2, 2);
                }
            }
        }
        else {
            var factor = Math.Max(0, Math.Min(1.0, Loudness));
            var width = bounds.Width * factor;
            int count = (int)(width / itemSize);
            if (count <= 0) return;

            for (int i = 0; i < count; i++) {
                var rect = new Rect(bounds.X + itemSize * i, bounds.Y, (itemSize - 3), bounds.Height);

                if (VolumeBrush is SolidColorBrush solid) {
                    dc.DrawRoundedRectangle(solid, null, rect, 2, 2);
                }
                else if (VolumeBrush is GradientBrush gradient) {
                    var color = gradient.GradientStops.GetRelativeColor(itemSize * i / bounds.Width);
                    var brush = new SolidColorBrush(color);
                    dc.DrawRoundedRectangle(brush, null, rect, 2, 2);
                }
            }
        }
    }

    private static void OnLoudnessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AudioIndicator ai) {
            ai.InvalidateVisual();
        }
    }
}
