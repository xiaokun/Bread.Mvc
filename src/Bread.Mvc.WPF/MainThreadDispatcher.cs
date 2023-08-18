using System.Windows;

namespace Bread.Mvc.WPF;


public class DispatcherPacker : IUIDispatcher
{
    public void Invoke(Action action)
    {
        Application.Current.Dispatcher.Invoke(action);
    }

    public Task InvokeAsync(Action action)
    {
        return Application.Current.Dispatcher.InvokeAsync(action).Task;
    }

    public Task<T> InvokeAsync<T>(Func<T> action)
    {
        return Application.Current.Dispatcher.InvokeAsync(action).Task;
    }

    public bool IsInvokeRequired
    {
        get {
            return Application.Current.CheckAccess() == false;
        }
    }
}
