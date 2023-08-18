using System.Windows;
using Point = System.Windows.Point;

namespace Bread.Mvc.WPF
{
	public class WindowMoveHelper
    {
        private Window _target;

        public WindowMoveHelper(Window window)
        {
            _target = window;
        }

        public void Attach(UIElement element)
        {
            element.MouseDown += Element_MouseDown;
            element.MouseMove += Element_MouseMove;
            element.MouseUp += Element_MouseUp;
        }

        private void Element_MouseUp(object? sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isMouseDown = false;
            var element = sender as UIElement;
            if (element == null) return;

            element.ReleaseMouseCapture();
        }

        private Point _mouseOrign = new Point(0, 0);
        private Point _windowOrign = new Point(0, 0);
        private bool _isMouseDown = false;

        private void Element_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var element = sender as UIElement;
            if (element == null) return;
            if (e.LeftButton != System.Windows.Input.MouseButtonState.Pressed) return;
          
            _mouseOrign = e.GetPosition(element);
            _mouseOrign = element.PointToScreen(_mouseOrign);
            _windowOrign = new Point(_target.Left, _target.Top);
            _isMouseDown = true;
            element.CaptureMouse();
        }

        private void Element_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!_isMouseDown) return;
            if (e.LeftButton != System.Windows.Input.MouseButtonState.Pressed) return;
            var element = sender as UIElement;
            if (element == null) return;

            var point = e.GetPosition(element);
            point = element.PointToScreen(point);
            double deltaX = point.X - _mouseOrign.X;
            double deltaY = point.Y - _mouseOrign.Y;

            _target.Left = _windowOrign.X + deltaX;
            _target.Top = _windowOrign.Y + deltaY;
        }
    }
}
