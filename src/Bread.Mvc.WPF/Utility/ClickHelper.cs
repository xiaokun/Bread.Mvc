using System;
using System.Windows;
using System.Windows.Threading;

namespace Bread.Mvc.WPF
{
    public class ClickHelper
    {
        public event Action<object?>? Click;

        public event Action<object?>? DoubleClick;

        public event Action<object?>? RightClick;

        readonly DispatcherTimer _timer = new();

        public ClickHelper(UIElement e, bool preview = false, int interval = 250)
        {
            if (preview) {
                e.PreviewMouseLeftButtonUp += E_PreviewMouseLeftButtonUp;
                e.PreviewMouseRightButtonDown += E_PreviewMouseRightButtonDown;
            }
            else {
                e.MouseLeftButtonUp += E_MouseLeftButtonUp;
                e.MouseRightButtonDown += E_MouseRightButtonDown;
            }

            _timer.Interval = TimeSpan.FromMilliseconds(interval);
            _timer.Tick += _timer_Tick;
        }

        private void E_MouseRightButtonDown(object? sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RightClick?.Invoke(e.OriginalSource);
        }

        private void E_PreviewMouseRightButtonDown(object? sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RightClick?.Invoke(e.OriginalSource);
        }

        private void _timer_Tick(object? sender, EventArgs e)
        {
            _timer.Stop();
            _clickCount = 0;
            Click?.Invoke(_sender);
        }

        private void E_PreviewMouseLeftButtonUp(object? sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LeftButtonDown(e.OriginalSource);
        }

        private void E_MouseLeftButtonUp(object? sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LeftButtonDown(e.OriginalSource);
        }

        object? _sender = null;
        int _clickCount = 0;

        void LeftButtonDown(object sender)
        {
            _clickCount++;
            _sender = sender;
            if (_clickCount == 2) {
                _timer.Stop();
                _clickCount = 0;
                DoubleClick?.Invoke(sender);
            }
            else {
                _timer.Start();
            }
        }
    }
}
