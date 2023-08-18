namespace Bread.Mvc;

/// <summary>
/// 弹出消息对话框，需要用户手动关闭
/// </summary>
public interface IMessageBox
{
    Task<bool?> ShowAsync(string title, string message, string additional, string ok, string cancel, string ignore);
}


public class MessageBox
{
    public static MessageBox Show { get; } = new MessageBox();

    private readonly IMessageBox _box;

    private MessageBox()
    {
        _box = IoC.Get<IMessageBox>();
    }

    public Task<bool?> Any(string title, string message, string additional, string ok, string cancel, string ignore)
    {
        return _box.ShowAsync(title, message, additional, ok, cancel, ignore);
    }

    /// <summary>
    /// 提示保存
    /// </summary>
    public Task<bool?> Save(string message, string additional = "")
    {
        return _box.ShowAsync("提醒", message, additional, "保存", "不保存", "取消");
    }

    /// <summary>
    /// 提示确认
    /// </summary>
    public Task<bool?> Confirm(string message, string additional = "")
    {
        return _box.ShowAsync("确认", message, additional, "是", "否", "");
    }
}
