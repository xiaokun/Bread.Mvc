using System.Windows;
using System.Windows.Controls.Primitives;

namespace Bread.Mvc.WPF;

public class AutoHidePopup : Popup
{
    private bool _isInitialized = false;
    private bool _showOnActive = false;
    private bool _isOpenBeforeWindowMinimized = false;
    private bool _showOnShown = false;

    public AutoHidePopup()
    {
    }

    public void Initialize()
    {
        if (_isInitialized) return;
        _isInitialized = true;
        var window = Application.Current.MainWindow;
        if (window == null) throw new Exception("只允许在主窗口初始化之后调用");
        window.LocationChanged += Windows_LocationChanged;
        window.SizeChanged += Windows_SizeChanged;
        window.Activated += Windows_Activated;
        window.Deactivated += Windows_Deactivated;
        window.StateChanged += Windows_StateChanged;
        window.IsVisibleChanged += Windows_IsVisibleChanged;
    }

    private void Windows_IsVisibleChanged(object? sender, DependencyPropertyChangedEventArgs e)
    {
        if (!((bool)e.NewValue)) {
            if (this.IsOpen) {
                _showOnShown = true;
                this.IsOpen = false;
            }
        }
        else {
            if (_showOnShown) {
                this.IsOpen = true;
                _showOnShown = false;
            }
        }
    }

    private void Windows_StateChanged(object? sender, EventArgs e)
    {
        var window = sender as Window;
        if (window == null) return;

        if (window.WindowState == WindowState.Minimized) {
            if (this.IsOpen) {
                _isOpenBeforeWindowMinimized = true;
                this.IsOpen = false;
            }
        }
        else {
            if (_isOpenBeforeWindowMinimized) {
                this.IsOpen = true;
            }
            _isOpenBeforeWindowMinimized = false;
        }
    }

    private void ResetPosition()
    {
        var offset = this.HorizontalOffset;
        this.HorizontalOffset = offset + 1;
        this.HorizontalOffset = offset;
    }

    private void Windows_Deactivated(object? sender, EventArgs e)
    {
        if (this.IsOpen) {
            _showOnActive = true;
            this.IsOpen = false;
        }
        else
            _showOnActive = false;
    }

    private void Windows_Activated(object? sender, EventArgs e)
    {
        if (_showOnActive) {
            this.IsOpen = true;
        }
    }

    private void Windows_SizeChanged(object? sender, System.Windows.SizeChangedEventArgs e)
    {
        ResetPosition();
    }

    private void Windows_LocationChanged(object? sender, EventArgs e)
    {
        ResetPosition();
    }
}
