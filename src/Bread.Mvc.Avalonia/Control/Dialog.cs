using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;

namespace Bread.Mvc.Avalonia;


public class InnerDialog : UserControl
{
    public bool IsBackgroundBlur { get; protected set; } = true;

    public bool? Result { get; set; } = null;

    private static int LayerIndex = 1500;
    Window? _window;

    public async Task<bool?> ShowAsync(Window owner)
    {
        if (double.IsNaN(Width)) throw new InvalidOperationException("Width not set");
        if (double.IsNaN(Height)) throw new InvalidOperationException("Height not set");

        if (_window != null) {
            _window.Close(false);
        }

        _window = new Window();
        _window.Background = new SolidColorBrush(Colors.Transparent);

        if (IsBackgroundBlur) {
            _window.TransparencyLevelHint = new List<WindowTransparencyLevel>() { WindowTransparencyLevel.Blur };
            _window.TransparencyBackgroundFallback = new SolidColorBrush(Color.FromArgb(0, 150, 150, 150));
        }
        else {
            _window.TransparencyLevelHint = new List<WindowTransparencyLevel>() { WindowTransparencyLevel.Blur };
            _window.TransparencyBackgroundFallback = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        _window.ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;
        _window.ExtendClientAreaTitleBarHeightHint = 0;
        _window.ExtendClientAreaToDecorationsHint = true;

        _window.CanResize = false;
        _window.BorderBrush = new SolidColorBrush(Colors.Transparent);
        _window.BorderThickness = new Thickness(0);
        _window.Focusable = false;
        _window.Topmost = owner.Topmost;

        _window.Width = owner.Width;
        _window.Height = owner.Height;
        _window.ShowInTaskbar = false;
        _window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

        if (IsBackgroundBlur) {
            _window.Content = new Canvas() {
                Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0))
            };
        }
        else {
            _window.Content = new Canvas() {
                Background = new SolidColorBrush(Color.FromArgb(10, 0, 0, 0))
            };
        }


        var canvas = _window.Content as Canvas;
        canvas!.Children.Add(this);
        Canvas.SetLeft(this, (owner.Bounds.Width - Width) / 2);
        Canvas.SetTop(this, (owner.Bounds.Height - Height) / 2);
        ZIndex = LayerIndex++;

        _ = await _window!.ShowDialog<bool>(owner);
        return Result;
    }

    public void Close()
    {
        _window?.Close(Result == true);
    }

    public void AttachDragMove(Border border)
    {
        border.PointerPressed += Border_PointerPressed;
        border.PointerMoved += Border_PointerMoved;
        border.PointerReleased += Border_PointerReleased;
    }

    bool _isMouseDown = false;
    Point _mouseDownPoint;
    Point _initDialogPoint;

    private void Border_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed == false) return;
        _isMouseDown = true;
        _mouseDownPoint = e.GetPosition(Parent as Visual);
        _initDialogPoint = this.Bounds.Position;
    }

    private void Border_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isMouseDown == false) return;
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed == false) return;

        var parent = Parent as Visual;
        if(parent == null) return;

        var p = e.GetPosition(parent);
        var x = _initDialogPoint.X + p.X - _mouseDownPoint.X;
        var y = _initDialogPoint.Y + p.Y - _mouseDownPoint.Y;

        //Log.Info($"point: {x},{y}\tDown:{_mouseDownPoint.X},{_mouseDownPoint.Y}\t Current: {p.X}, {p.Y}");

        if (x < 0 || y < 0) return;

        if (x + Bounds.Width > parent.Bounds.Width) return;
        if (y + Bounds.Height > parent.Bounds.Height) return;

        Canvas.SetLeft(this, x);
        Canvas.SetTop(this, y);
    }

    private void Border_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isMouseDown = false;
    }
}