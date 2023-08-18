using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Bread.Mvc.WPF;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
public class VideoPanel : Panel
{
    public event HwndSourceHook? OnMessage;
    private SolidBrush _brush;
    private bool _isNeedPaint = true;

    public VideoPanel()
    {
        _brush = new SolidBrush(Color.Black);
        SetStyle(ControlStyles.UserPaint, true);
        SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
        SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
        this.DoubleBuffered = true;
    }

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        bool handled = false;
        OnMessage?.Invoke(m.HWnd, m.Msg, m.WParam, m.LParam, ref handled);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        if (Width <= 0 || Height <= 0) return;
        if (_isNeedPaint == false) return;
        var rect = new Rectangle(0, 0, this.Width, this.Height);
        e.Graphics.FillRectangle(_brush, rect);
        _isNeedPaint = false;
    }

    protected override void OnBackColorChanged(EventArgs e)
    {
        base.OnBackColorChanged(e);
        _brush?.Dispose();
        _brush = new SolidBrush(this.BackColor);
        _isNeedPaint = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
    }
}
