using Avalonia.Threading;

namespace Bread.Mvc.Avalonia;

public class MainThreadDispatcher : IUIDispatcher
{
    private Dispatcher? _dispatcher;

    public void Invoke(Action action)
    {
        _dispatcher ??= Dispatcher.UIThread;
        Dispatcher.UIThread.Post(action);
    }

    public Task InvokeAsync(Action action)
    {
        _dispatcher ??= Dispatcher.UIThread;
        return Dispatcher.UIThread.InvokeAsync(action).GetTask();
    }

    public Task<T> InvokeAsync<T>(Func<T> action)
    {
        _dispatcher ??= Dispatcher.UIThread;
        return Dispatcher.UIThread.InvokeAsync(action).GetTask();
    }

    public bool IsInvokeRequired
    {
        get {
            _dispatcher ??= Dispatcher.UIThread;
            return _dispatcher.CheckAccess() == false;
        }
    }
}