using System.ComponentModel;

namespace Bread.Utility.Threading;

public class AsyncState : INotifyPropertyChanged
{
    public bool IsCompleted { get; set; } = false;

    public bool IsCanceled { get; set; } = false;

    public bool IsStarted { get; set; } = false;

    public bool IsPaused { get; set; } = false;

    public int Percent { get; set; } = 0;

    public string Title { get; set; } = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

public interface IAsyncWorkItem
{
    string Tag { get; }

    AsyncState State { get; }

    void StartAsync();

    void PauseAsync();

    void ResumeAsync();

    void CancelAsync();

}


public class AsyncActionItem : IAsyncWorkItem
{
    public string Tag { get; private set; }

    public AsyncState State { get; private set; } = new AsyncState();

    private Action<AsyncState> _action;

    public AsyncActionItem(Action<AsyncState> action, string tag = "Action")
    {
        _action = action;
        Tag = tag;
    }


    public void CancelAsync()
    {
        State.IsCanceled = true;
    }

    public void PauseAsync()
    {
        State.IsPaused = true;
    }

    public void ResumeAsync()
    {
        State.IsPaused = false;
    }

    public void StartAsync()
    {
        State.IsStarted = true;
        ThreadPool.QueueUserWorkItem((o) => {
            _action(o as AsyncState ?? new AsyncState());
            State.IsCompleted = true;
            State.IsPaused = false;
            State.IsCanceled = false;
        }, State);

    }

}
