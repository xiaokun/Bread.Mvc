using System.Collections.ObjectModel;
using Bread.Utility.IO;

namespace Bread.Utility.Threading;

public class AsyncWorkQueue : IDisposable
{
    public event Action<Exception>? Interrupted;

    /// <summary>
    /// 最大并发数
    /// </summary>
    public int MaxParallerCount { get; set; } = 1;

    List<IAsyncWorkItem> _items = new List<IAsyncWorkItem>();

    public ReadOnlyCollection<IAsyncWorkItem> Items { get { return _items.AsReadOnly(); } }

    object _itemsLocker = new();

    Thread _thread;
    AutoResetEvent _waitTaskEvent = new(false);
    AutoResetEvent _waitForCloseEvent = new(false);

    public AsyncWorkQueue()
    {
        _thread = new Thread(new ThreadStart(ThreadMain));
        _thread.Start();
    }

    private bool _isClosed = false;
    public void Close()
    {
        if (_isClosed) return;

        Clear();

        _waitTaskEvent.Set();
        _isClosed = true;
        _waitForCloseEvent.WaitOne(1000);
    }

    public void QueueWork(IAsyncWorkItem item, Action<IList<IAsyncWorkItem>>? action = null)
    {
        lock (_itemsLocker) {
            _items.Add(item);
            if (action != null) action(_items);
        }

        _waitTaskEvent.Set();
    }


    public void Cancle(Action<List<IAsyncWorkItem>> cancler)
    {
        if (cancler == null) {
            lock (_itemsLocker) {
                foreach (var item in _items) {
                    item.CancelAsync();
                }
            }
            return;
        }

        lock (_itemsLocker) {
            cancler(_items);
        }
    }

    public void Pause()
    {
        lock (_itemsLocker) {
            foreach (var item in _items) {
                if (item.State.IsStarted) {
                    item.PauseAsync();
                }
            }
        }
    }

    public void Resume()
    {
        lock (_itemsLocker) {
            foreach (var item in _items) {
                if (item.State.IsPaused) {
                    item.ResumeAsync();
                }
            }
        }
    }


    public void Clear()
    {
        lock (_itemsLocker) {
            foreach (var item in _items) {
                item.CancelAsync();
            }
            _items.Clear();
        }
    }

    private void ThreadMain()
    {
        int waitTime = 100;
        try {
            while (true) {
                _waitTaskEvent.WaitOne(waitTime);

                // run
                if (_isClosed) {
                    _waitForCloseEvent.Set();
                    return;
                }

                //task
                if (_items.Count == 0) {
                    waitTime = 600;
                    continue;
                }

                lock (_itemsLocker) {
                    for (int i = 0; i < MaxParallerCount;) {
                        if (i == _items.Count) break;
                        if (_items.Count == 0) break;

                        var item = _items[i];
                        if (item.State.IsCanceled || item.State.IsCompleted) {
                            _items.RemoveAt(i);
                            continue;
                        }

                        if (!item.State.IsStarted) {
                            item.StartAsync();
                        }

                        i++;
                    }
                }

                waitTime = 100;
            }
        }
        catch (Exception ex) {
            Interrupted?.Invoke(ex);
        }
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue) {
            Close();
            _waitTaskEvent.Dispose();
            _waitForCloseEvent.Dispose();
            disposedValue = true;
        }
    }

    ~AsyncWorkQueue()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion

}