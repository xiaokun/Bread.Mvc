namespace Bread.Mvc;

/// <summary>
/// 弹出提示信息，自动消失，可弹出多个
/// </summary>
public interface IAlertBox
{
    void ShowInfo(string msg, TimeSpan? expirationTime = null, Action? onClick = null, Action? onClose = null);

    void ShowSuccess(string msg, TimeSpan? expirationTime = null, Action? onClick = null, Action? onClose = null);

    void ShowWarning(string msg, TimeSpan? expirationTime = null, Action? onClick = null, Action? onClose = null);

    void ShowError(string msg, TimeSpan? expirationTime = null, Action? onClick = null, Action? onClose = null);
}


public class AlertBox
{
    public static AlertBox Show { get; } = new AlertBox();

    IAlertBox? _box = null;

    private AlertBox()
    {
        try { _box = IoC.Get<IAlertBox>(); } catch { }
    }

    public void Info(string msg, TimeSpan? time = null,
        Action? onClick = null, Action? onClose = null)
    {
        _box?.ShowInfo(msg, time ?? TimeSpan.FromSeconds(3), onClick, onClose);
    }

    public void Success(string msg, TimeSpan? time = null,
        Action? onClick = null, Action? onClose = null)
    {
        _box?.ShowSuccess(msg, time ?? TimeSpan.FromSeconds(3), onClick, onClose);
    }

    public void Warning(string msg, TimeSpan? time = null,
        Action? onClick = null, Action? onClose = null)
    {
        _box?.ShowWarning(msg, time ?? TimeSpan.FromSeconds(5), onClick, onClose);
    }

    public void Error(string msg, TimeSpan? time = null,
        Action? onClick = null, Action? onClose = null)
    {
        _box?.ShowError(msg, time ?? TimeSpan.FromMinutes(1), onClick, onClose);
    }

    public void Fatal(string msg, TimeSpan? time = null,
        Action? onClick = null, Action? onClose = null)
    {
        _box?.ShowError(msg, time ?? TimeSpan.MaxValue, onClick, onClose);
    }
}
