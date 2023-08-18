using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Point = System.Windows.Point;

namespace Bread.Mvc.WPF;

internal partial class ContentHostWindow : Window
{
    Window? _hostWnd;
    readonly Point _zeroPoint = new Point(0, 0);
    readonly VideoView _host;

    public ContentHostWindow(VideoView host)
    {
        Height = 300;
        Width = 300;

        this.ShowInTaskbar = false;
        this.WindowStyle = WindowStyle.None;
        this.ResizeMode = ResizeMode.NoResize;
        this.Background = new SolidColorBrush(Colors.Transparent);

        var chrome = new WindowChrome();
        chrome.GlassFrameThickness = new Thickness(-1);
        chrome.CaptionHeight = 0;
        chrome.ResizeBorderThickness = new Thickness(0);
        WindowChrome.SetWindowChrome(this, chrome);

        DataContext = host.DataContext;

        _host = host;
        _host.DataContextChanged += __host_DataContextChanged;
        _host.Loaded += host_Loaded;
        _host.Unloaded += host_Unloaded;
        _host.IsVisibleChanged += __host_IsVisibleChanged;
    }

    private void __host_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (this.IsVisible == _host.IsVisible) return;
        if (_host.IsVisible) Show();
        else Hide();
    }

    private void __host_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        DataContext = e.NewValue;
    }

    void host_Loaded(object sender, RoutedEventArgs e)
    {
        if (_hostWnd != null && IsVisible) return;

        _hostWnd = GetWindow(_host);
        if (_hostWnd == null) return;

        Owner = _hostWnd;

        _hostWnd.Closing += Wndhost_Closing;
        _hostWnd.LocationChanged += UpdateSize;
        //_host.LayoutUpdated += UpdateSize;
        _host.SizeChanged += UpdateSize;

        UpdateSize(null, null);

        Show();
        _hostWnd.Focus();
    }


    void host_Unloaded(object sender, RoutedEventArgs e)
    {
        //_host.LayoutUpdated -= UpdateSize;
        _host.SizeChanged -= UpdateSize;
        if (_hostWnd != null) {
            _hostWnd.Closing -= Wndhost_Closing;
            _hostWnd.LocationChanged -= UpdateSize;
        }
        Hide();
    }

    internal void UpdateSize(object? sender, EventArgs? e)
    {
        if (_host == null) return;
        if (_hostWnd == null) return;

        var source = PresentationSource.FromVisual(_hostWnd);
        if (source == null) {
            return;
        }

        try {
            var locationFromScreen = _host.PointToScreen(_zeroPoint);
            var targetPoints = source.CompositionTarget.TransformFromDevice.Transform(locationFromScreen);
            if (double.IsNaN(Left) || Math.Abs(Left - targetPoints.X) > 0.5) Left = targetPoints.X;
            if (double.IsNaN(Top) || Math.Abs(Top - targetPoints.Y) > 0.5) Top = targetPoints.Y;

            var box = WPFHelper.GetParentOfType<Viewbox>(_host);
            if (box == null || (box.Child is not FrameworkElement)) {
                if (Math.Abs(Width - _host.ActualWidth) > 0.5) Width = _host.ActualWidth;
                if (Math.Abs(Height - _host.ActualHeight) > 0.5) Height = _host.ActualHeight;
            }
            else {
                if (box.Child is FrameworkElement fe) {
                    var xf = box.ActualWidth / fe.ActualWidth * _host.ActualWidth;
                    var yf = box.ActualHeight / fe.ActualHeight * _host.ActualHeight;
                    if (Math.Abs(Width - xf) > 0.5) Width = xf;
                    if (Math.Abs(Height - xf) > 0.5) Height = yf;
                }
            }
        }
        catch (Exception ex) {
            Hide();
            throw new Exception("Unable to create WPF Window in VideoView.", ex);
        }
    }

    void Wndhost_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        Close();

        _host.DataContextChanged -= __host_DataContextChanged;
        _host.Loaded -= host_Loaded;
        _host.Unloaded -= host_Unloaded;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.System && e.SystemKey == Key.F4) {
            _hostWnd?.Focus();
        }
    }
}
