using Avalonia.Input;
using Avalonia.Threading;

namespace Bread.Mvc.Avalonia;

public class ClickHelper
{
    public event Action<object?>? Click;
    public event Action<object?>? DoubleClick;
    public event Action<object?>? RightClick;

    readonly DispatcherTimer _timer = new();

    public ClickHelper(InputElement e, int interval = 250)
    {
        e.PointerReleased += E_PointerReleased; // mouse up
        _timer.Interval = TimeSpan.FromMilliseconds(interval);
        _timer.Tick += _timer_Tick;
    }

    object? _sender = null;
    int _clickCount = 0;
    private void E_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == MouseButton.Right) {
            RightClick?.Invoke(e.Source);
            return;
        }

        _clickCount++;
        _sender = e.Source;
        if (_clickCount == 2) {
            _timer.Stop();
            _clickCount = 0;
            DoubleClick?.Invoke(_sender);
        }
        else {
            _timer.Start();
        }
    }

    private void _timer_Tick(object? sender, EventArgs e)
    {
        _timer.Stop();
        _clickCount = 0;
        Click?.Invoke(_sender);
    }
}