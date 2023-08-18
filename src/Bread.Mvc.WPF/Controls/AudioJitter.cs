using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Orientation = System.Windows.Controls.Orientation;
using Panel = System.Windows.Controls.Panel;

namespace Bread.Mvc.WPF;

/// <summary>
/// 用于显示音量或电池容量，长度分成10格
/// </summary>
public class AudioJitter : Panel
{
    public Orientation Orientation
    {
        set { SetValue(OrientationProperty, value); }
        get { return (Orientation)GetValue(OrientationProperty); }
    }
    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register("Orientation", typeof(Orientation), typeof(AudioJitter),
            new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnPropertyChangedCallback));


    public Brush OffBrush
    {
        set { SetValue(OffBrushProperty, value); }
        get { return (Brush)GetValue(OffBrushProperty); }
    }
    public static readonly DependencyProperty OffBrushProperty =
        DependencyProperty.Register("OffBrush", typeof(Brush), typeof(AudioJitter),
            new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnPropertyChangedCallback));

    public Brush OnBrush
    {
        set { SetValue(OnBrushProperty, value); }
        get { return (Brush)GetValue(OnBrushProperty); }
    }
    public static readonly DependencyProperty OnBrushProperty =
        DependencyProperty.Register("OnBrush", typeof(Brush), typeof(AudioJitter),
            new FrameworkPropertyMetadata(Brushes.BlueViolet, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnPropertyChangedCallback));

    /// <summary>
    /// 音量值，0~100
    /// </summary>
    public int Volume
    {
        set { SetValue(VolumeProperty, value); }
        get { return (int)GetValue(VolumeProperty); }
    }
    public static readonly DependencyProperty VolumeProperty =
        DependencyProperty.Register("Volume", typeof(int), typeof(AudioJitter),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnPropertyChangedCallback));

    public int Nub
    {
        set { SetValue(NubProperty, value); }
        get { return (int)GetValue(NubProperty); }
    }
    public static readonly DependencyProperty NubProperty =
        DependencyProperty.Register("Nub", typeof(int), typeof(AudioJitter),
            new FrameworkPropertyMetadata(6, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnPropertyChangedCallback));

    public int Gap
    {
        set { SetValue(GapProperty, value); }
        get { return (int)GetValue(GapProperty); }
    }
    public static readonly DependencyProperty GapProperty =
        DependencyProperty.Register("Gap", typeof(int), typeof(AudioJitter),
            new FrameworkPropertyMetadata(2, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnPropertyChangedCallback));

    private static void OnPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var jitter = d as AudioJitter;
        if (jitter == null) return;
        jitter.InvalidateVisual();
    }

    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);

        if (double.IsNaN(this.ActualWidth)) return;
        if (double.IsNaN(this.ActualHeight)) return;
        if (this.ActualHeight < 20) return;

        int volume = Volume;
        volume = Math.Max(0, Math.Min(100, volume));

        if (Orientation == Orientation.Vertical) {
            DrawVertical(dc, volume);
        }
        else {
            DrawHorizontal(dc, volume);
        }
    }

    private void DrawVertical(DrawingContext dc, int volume)
    {
        var gap = Gap;
        var nub = Nub;
        int count = (int)((this.ActualHeight + gap) / (double)(gap + nub));
        var delta = (this.ActualHeight - count * (gap + nub)) / 2;

        for (int i = 0; i < count; i++) {
            double y = i * (gap + nub) + delta;
            var rect = new Rect(0, y, this.ActualWidth, nub);
            dc.DrawRectangle(OffBrush, null, rect);
        }

        var value = (100 - volume) / 100.0;
        var height = value * count * (gap + nub);
        for (int i = 0; i < count; i++) {
            double y = i * (gap + nub) + delta;
            if (y + nub < height) continue;

            var rect = new Rect(0, (int)y, (int)this.ActualWidth, nub);
            if (y < height) {
                rect = new Rect(0, (int)height, (int)this.ActualWidth, (int)(y + nub - height));
            }

            dc.DrawRectangle(OnBrush, null, rect);
        }
    }

    private void DrawHorizontal(DrawingContext dc, int volume)
    {

    }
}