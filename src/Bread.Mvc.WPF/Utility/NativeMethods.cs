using System;
using System.Runtime.InteropServices;

namespace Bread.Mvc.WPF;

internal static class NativeMethods
{
    [DllImport("user32.dll")]
    internal static extern int GetSystemMetrics(SM nIndex);


    public const int GWL_STYLE = -16;
    public const long WS_POPUP = 0x80000000L;
    public const long WS_CAPTION = 0x00C00000L;  /* WS_BORDER | WS_DLGFRAME  */
    public const long WS_THICKFRAME = 0x00040000L;
    public const long WS_CHILD = 0x40000000L;

    public const uint SWP_NOSIZE = 0x0001;
    public const uint SWP_NOMOVE = 0x0002;
    public const uint SWP_NOACTIVATE = 0x0010;


    [DllImport("user32")]
    public static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

    [DllImport("user32.dll")]
    public static extern int MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    [DllImport("User32.dll", EntryPoint = "GetWindowLong")]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);


    [DllImport("User32.dll", EntryPoint = "SetWindowLong")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
}
