using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Interop;
using System.Windows.Media;
using Timer = System.Timers.Timer;

namespace Bread.Mvc.WPF;

[TemplatePart(Name = PART_PlayerHost, Type = typeof(WindowsFormsHost))]
[TemplatePart(Name = PART_PlayerView, Type = typeof(VideoPanel))]
public class VideoView : ContentControl, IDisposable
{
    private const string PART_PlayerHost = "PART_PlayerHost";
    private const string PART_PlayerView = "PART_PlayerView";

    WindowsFormsHost? _formHost = null;
    VideoPanel? _formPanel = null;

    public event Action? Click;
    public event Action? LeftButtonDown;
    public event Action? LeftButtonUp;
    public event Action? LeftButtonMove;
    public event Action? DoubleClicks;
    public event Action? RightClick;
    public event HwndSourceHook? OnMessage;
    public event Action? HwndInited;

    private Timer? _timer = null;
    private Form? _fullScreenForm = null;

    public bool UseContentWindow
    {
        get { return (bool)GetValue(UseContentWindowProperty); }
        set { SetValue(UseContentWindowProperty, value); }
    }

    public static readonly DependencyProperty UseContentWindowProperty = DependencyProperty.Register(
        "UseContentWindow", typeof(bool), typeof(VideoView), new PropertyMetadata(false));


    public bool IsContentWindowVisable
    {
        get { return (bool)GetValue(IsContentWindowVisableProperty); }
        set { SetValue(IsContentWindowVisableProperty, value); }
    }

    public static readonly DependencyProperty IsContentWindowVisableProperty = DependencyProperty.Register(
        "IsContentWindowVisable", typeof(bool), typeof(VideoView), new PropertyMetadata(true, OnIsContentWindowVisableChagned));

    public VideoView()
    {
        DefaultStyleKey = typeof(VideoView);
        _timer = new Timer() { Interval = this.IntervalTime };
        _timer.Elapsed += _timer_Tick; ;
        this.IsVisibleChanged += VideoView_IsVisibleChanged;
        this.IsEnabledChanged += VideoView_IsEnabledChanged;
    }

    private void VideoView_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (_formHost == null) return;
        if (_formPanel == null) return;

        if (this.IsEnabled) {
            _formHost.IsEnabled = true;
            _formPanel.Enabled = true;
            if (_contentHost != null) _contentHost.IsEnabled = true;
        }
        else {
            _formHost.IsEnabled = false;
            _formPanel.Enabled = false;
            if (_contentHost != null) _contentHost.IsEnabled = false;
        }
    }

    private void VideoView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (_formHost == null) return;
        if (_formPanel == null) return;

        if (this.IsVisible) {
            _formHost.Visibility = Visibility.Visible;
            _formPanel.Visible = true;
            if (_contentHost != null) {
                if (IsContentWindowVisable) _contentHost.Visibility = Visibility.Visible;
                else _contentHost.Visibility = Visibility.Hidden;
            }
        }
        else {
            _formHost.Visibility = Visibility.Hidden;
            _formPanel.Visible = false;
            if (_contentHost != null) _contentHost.Visibility = Visibility.Hidden;
        }
    }

    private void FullScreenForm_KeyUp(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape) {
            ExitFullScreen();
        }
    }

    private bool _isFullScreened = false;
    public void ShowFullScreen()
    {
        if (_fullScreenForm != null) return;

        _fullScreenForm = new Form();
        _fullScreenForm.BackColor = System.Drawing.Color.Black;
        _fullScreenForm.ClientSize = new System.Drawing.Size(533, 300);
        _fullScreenForm.ControlBox = false;
        _fullScreenForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        _fullScreenForm.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
        _fullScreenForm.MaximizeBox = false;
        _fullScreenForm.MinimizeBox = false;
        _fullScreenForm.Name = "全屏窗口";
        _fullScreenForm.ShowIcon = false;
        _fullScreenForm.ShowInTaskbar = false;
        _fullScreenForm.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
        _fullScreenForm.Text = "全屏窗口";
        _fullScreenForm.TopMost = true;
        _fullScreenForm.WindowState = System.Windows.Forms.FormWindowState.Maximized;
        _fullScreenForm.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FullScreenForm_KeyUp);

        _isFullScreened = true;
        if (_formPanel != null) {
            _formPanel.OnMessage -= _formPanel_OnMessage;
        }
       
        if(_formHost != null) {
            _formHost.Child = null;
        }

        _fullScreenForm.Controls.Add(_formPanel);

        if (_formPanel != null) {
            _formPanel.Dock = DockStyle.Fill;
        }
     
        _fullScreenForm.Show();
    }

    public void UpdateOverlaySize()
    {
        if (_contentHost == null) return;
        if (IsVisible == false) return;
        if (_contentHost.IsVisible == false) return;
        _contentHost.UpdateSize(null, null);
    }

    private void _fullScreenForm_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        ExitFullScreen();
    }

    private void ExitFullScreen()
    {
        if (_fullScreenForm == null) return;

        _fullScreenForm.Hide();
        _fullScreenForm.Controls.Remove(_formPanel);
        if (_formHost != null) {
            _formHost.Child = _formPanel;
        }
      
        _isFullScreened = false;

        if(_formPanel != null) {
            _formPanel.OnMessage += _formPanel_OnMessage;
        }

        _fullScreenForm.Close();
        _fullScreenForm = null;
    }

    bool _isFirstClick = true;
    bool _isDoubleClick = false;
    int _milliseconds = 0;
    int DoubleClickTime = 320;
    int IntervalTime = 40;

    private void _timer_Tick(object? sender, EventArgs e)
    {
        _milliseconds += IntervalTime;

        // The timer has reached the double click time limit.
        if (_milliseconds >= DoubleClickTime || _isDoubleClick) {
            _timer?.Stop();
            if (_isDoubleClick) {
                if (_isFullScreened) ExitFullScreen();
                else DoubleClicks?.Invoke();
            }
            else {
                Click?.Invoke();
            }

            // Allow the MouseDown event handler to process clicks again.
            _isFirstClick = true;
            _isDoubleClick = false;
            _milliseconds = 0;

        }
    }

    private void _formPanel_MouseDown(object? sender, MouseEventArgs e)
    {
        if (((int)e.Button & (int)System.Windows.Forms.MouseButtons.Left) == (int)System.Windows.Forms.MouseButtons.Left) {
            LeftButtonDown?.Invoke();
        }
    }

    private void _formPanel_MouseMove(object? sender, MouseEventArgs e)
    {
        LeftButtonMove?.Invoke();
    }

    private void WinformPanel_MouseUp(object? sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (((int)e.Button & (int)System.Windows.Forms.MouseButtons.Left) == (int)System.Windows.Forms.MouseButtons.Left) {
            if (LeftButtonUp != null) {
                LeftButtonUp.Invoke();
                return;
            }

            if (_isFirstClick) {
                _isFirstClick = false;
                _timer?.Start();
                _milliseconds = 0;
            }
            else {
                if (_milliseconds < DoubleClickTime) {
                    _isDoubleClick = true;
                }
            }

        }
        else if (((int)e.Button & (int)System.Windows.Forms.MouseButtons.Right) == (int)System.Windows.Forms.MouseButtons.Right) {
            RightClick?.Invoke();
            _isFirstClick = true;
            _isDoubleClick = false;
            _milliseconds = 0;
            if (_timer?.Enabled ?? false) _timer?.Stop();
        }
    }

    private bool IsDesignMode => (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;

    private ContentHostWindow? _contentHost { get; set; }

    private bool IsUpdatingContent { get; set; }

    private UIElement? ViewContent { get; set; }

    public IntPtr Hwnd { get; set; }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (IsDesignMode) return;

        _formHost = Template.FindName(PART_PlayerHost, this) as WindowsFormsHost;
        if (_formHost != null) {
            if (UseContentWindow) {
                _contentHost = new ContentHostWindow(this) {
                    Content = ViewContent
                };
            }
        }

        if (_formPanel != null) {
            _formPanel.MouseDown -= _formPanel_MouseDown;
            _formPanel.MouseUp -= WinformPanel_MouseUp;
            _formPanel.MouseMove -= _formPanel_MouseMove;
            _formPanel.OnMessage -= _formPanel_OnMessage;
        }

        var panel = Template.FindName(PART_PlayerView, this) as VideoPanel;
        if (panel == null) return;

        _formPanel = panel;
        Hwnd = _formPanel!.Handle;
        if (Hwnd == IntPtr.Zero) {
            Trace.WriteLine("HWND is NULL, aborting...");
            return;
        }
        if (this.Background is SolidColorBrush brush) {
            var color = brush.Color;
            _formPanel!.BackColor = System.Drawing.Color.FromArgb(color.ToInt32());
        }

        _formPanel.OnMessage += _formPanel_OnMessage;
        _formPanel.MouseUp += WinformPanel_MouseUp;
        _formPanel.MouseMove += _formPanel_MouseMove;
        _formPanel.MouseDown += _formPanel_MouseDown;

        this.HwndInited?.Invoke();
    }

    private IntPtr _formPanel_OnMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        OnMessage?.Invoke(hwnd, msg, wParam, lParam, ref handled);
        return IntPtr.Zero;
    }

    private static void OnIsContentWindowVisableChagned(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
        if (dp is not VideoView vv) return;
        if (vv == null) return;
        if (vv.UseContentWindow == false) {
            return;
        }

        if (vv.Visibility == Visibility.Visible) {
            if (vv.IsContentWindowVisable && vv._contentHost != null) {
                vv._contentHost.Visibility = Visibility.Visible;
                return;
            }
        }

        if(vv._contentHost != null) {
            vv._contentHost.Visibility = Visibility.Hidden;
        }
    }

    protected override void OnContentChanged(object oldContent, object newContent)
    {
        base.OnContentChanged(oldContent, newContent);

        if (IsDesignMode || IsUpdatingContent) {
            return;
        }

        IsUpdatingContent = true;
        try {
            Content = null;
        }
        finally {
            IsUpdatingContent = false;
        }

        var content = newContent as UIElement;
        if (content == null) return;
        ViewContent = content;

        if (_contentHost != null) {
            _contentHost.Content = ViewContent;
        }
    }

    #region IDisposable Support

    bool disposedValue;
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue) {
            if (disposing) {
            }

            ViewContent = null;
            _contentHost = null;

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }

    #endregion
}
