namespace Bread.Mvc;

/// <summary>
/// 用于将任务分发到UI线程执行
/// </summary>
public interface IUIDispatcher
{
    bool IsInvokeRequired { get; }

    void Invoke(Action action);

    Task InvokeAsync(Action action);

    Task<T> InvokeAsync<T>(Func<T> action);
}
