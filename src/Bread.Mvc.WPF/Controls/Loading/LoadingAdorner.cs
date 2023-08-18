using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Bread.Mvc.WPF;

public class LoadingAdorner : Adorner
{
    private bool _isVisable = false;
    private System.Timers.Timer _timer = new System.Timers.Timer();
    private double value = 0;

    public LoadingAdorner(UIElement parent)
        : base(parent)
    {
        _timer.Interval = 25;
        _timer.Elapsed += _timer_Elapsed;
        _timer.AutoReset = true;
    }

    ~LoadingAdorner()
    {
        if (_timer == null) return;
        _timer.Stop();
        _timer.Dispose();
    }

    private void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        if (!_isVisable) return;
        if (value < AdornedElement.DesiredSize.Width) {
            value += 5;
        }
        else
            value = 0;
    }

    public void Show()
    {
        if (_isVisable) return;
        _isVisable = true;
        this.Width = this.AdornedElement.DesiredSize.Width;
        this.Height = this.AdornedElement.DesiredSize.Height;
        AdornerLayer layer = AdornerLayer.GetAdornerLayer(this.AdornedElement);
        layer.Add(this);
        _timer.Start();
    }

    public void Hide()
    {
        if (!_isVisable) return;
        _isVisable = false;
        AdornerLayer layer = AdornerLayer.GetAdornerLayer(this.AdornedElement);
        layer.Remove(this);
        _timer.Stop();
    }

    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);
        //TODO: loading adorner
        SolidColorBrush bkBrush = new SolidColorBrush(Color.FromArgb(100, 23, 23, 23));
        SolidColorBrush boderBrush = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));
        double scale = value / AdornedElement.DesiredSize.Width;
        double height = AdornedElement.DesiredSize.Width * scale;
        double x = (AdornedElement.DesiredSize.Width - value) / 2;
        double y = (AdornedElement.DesiredSize.Height - height) / 2;
        Rect rect = new Rect(x, y, value, height);
        dc.DrawRoundedRectangle(bkBrush, new Pen(boderBrush, 2), rect, 5, 5);
    }

}
