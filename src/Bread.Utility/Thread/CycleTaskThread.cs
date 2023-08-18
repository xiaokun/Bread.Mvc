using Bread.Utility.IO;

namespace Bread.Utility.Threading;

public class CycleTaskThread : IDisposable
{
    volatile bool _isRunning = false;
    public bool IsRuning => _isRunning;

    volatile bool _isCanceling = false;
    int _cancelWaittingTime = int.MaxValue;

    Func<int> _action;
    Thread _thread;
    AutoResetEvent _waitForExitEvent = new AutoResetEvent(false);
    AutoResetEvent _waiter = new AutoResetEvent(false);

    public CycleTaskThread(Func<int> action, int cancelWaittingTime = int.MaxValue)
    {
        _action = action;
        _isRunning = true;
        _cancelWaittingTime = cancelWaittingTime;

        _thread = new Thread(CycleTaskThreadProc);
        _thread.Start();
    }

    private void Cancel()
    {
        if (_isCanceling) return;

        if (IsRuning == false) {
            return;
        }

        _isCanceling = true;
        _waiter.Set();
        if (_waitForExitEvent.WaitOne(_cancelWaittingTime) == false) {
            Log.Error($"{nameof(CycleTaskThread)} cancel wait timeout");
        }
    }

    protected void CycleTaskThreadProc(object? state)
    {
        while (true) {
            if (_isCanceling) {
                _waitForExitEvent?.Set();
                break;
            }

            if (_action != null) {
                var result = _action();
                if (result < 0) {
                    break;
                }
                if (result > 0) {
                    _waiter.WaitOne(result);
                }
            }
        }
        _isRunning = false;
    }

    #region IDisposable

    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue) {
            if (IsRuning) {
                Cancel();
            }

            _waitForExitEvent.Dispose();
            _waiter?.Dispose();
            disposedValue = true;
        }
    }

    ~CycleTaskThread()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
