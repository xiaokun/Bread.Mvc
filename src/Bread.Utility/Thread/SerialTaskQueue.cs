﻿using System.Collections.Concurrent;
using Bread.Utility.IO;

namespace Bread.Utility.Threading;

internal struct SerialTaskQueueAction
{
    public string? Name;
    public Action Action;

    public SerialTaskQueueAction(string? name, Action action)
    {
        Name = name;
        Action = action;
    }
}

/// <summary>
/// 串行任务队列
/// 任务在单线程中按添加顺序依次执行
/// </summary>
public class SerialTaskQueue : IDisposable
{
    /// <summary>
    /// 睡眠时间
    /// </summary>
    public int SleepTime { get; set; } = 40;

    public bool IsStarted => _isStarted;

    ConcurrentQueue<SerialTaskQueueAction> _tasks;
    ConcurrentQueue<SerialTaskQueueAction> _topTasks;

    bool _isStarted = false;
    bool _isCanceled = false;
    string? _moduleName = null;

    SemaphoreSlim semaphore;
    AutoResetEvent _waitEvent = new AutoResetEvent(false);
    AutoResetEvent _cancelEvent = new AutoResetEvent(false);
    private bool disposedValue;

    /// <summary>
    /// SerialTaskQueue 构造函数
    /// </summary>
    /// <param name="moduleName">模块名称，用于输出日志</param>
    /// <param name="sleepTime">空闲时循环等待时间，单位毫秒</param>
    public SerialTaskQueue(string? moduleName = null)
    {
        _moduleName = moduleName ?? String.Empty;
        _tasks = new ConcurrentQueue<SerialTaskQueueAction>();
        _topTasks = new ConcurrentQueue<SerialTaskQueueAction>();
        semaphore = new SemaphoreSlim(1);
    }

    public void Start()
    {
        if (_isStarted) return;
        _isCanceled = false;
        _isStarted = true;
        ThreadPool.QueueUserWorkItem(new WaitCallback(MainThread));
    }

    /// <summary>
    /// 插入一个任务到队列末尾
    /// </summary>
    /// <param name="task"></param>
    /// <param name="name"></param>
    public void QueueTask(Action task, string? name = null)
    {
        if (!_isStarted || _isCanceled) return;
        _tasks.Enqueue(new(name, task));
        _waitEvent.Set();
    }

    /// <summary>
    /// 插入一个任务到队列开头，优先执行插队的任务
    /// </summary>
    /// <param name="task"></param>
    /// <param name="name"></param>
    public void InsertTask(Action task, string? name = null)
    {
        if (!_isStarted || _isCanceled) return;
        _topTasks.Enqueue(new(name, task));
        _waitEvent.Set();
    }

    private void MainThread(object? o)
    {
        try {
            while (true) {
                if (_isCanceled && _tasks.Count <= 0 && _topTasks.Count <= 0) {
                    _cancelEvent.Set();
                    return;
                }

                while (_topTasks.TryDequeue(out SerialTaskQueueAction toptask)) {
                    //await semaphore.WaitAsync();
                    try {
                        toptask.Action();
                    }
                    catch (Exception ex) {
                        Log.Error($"action:{toptask.Name ?? String.Empty} exe failed: {ex.Message}");
                        Log.Exception(ex);
                    }
                    //semaphore.Release();
                    continue;
                }

                while (_tasks.TryDequeue(out SerialTaskQueueAction task)) {
                    RunTaskAsync(task);
                    continue;
                }

                _waitEvent.WaitOne(SleepTime);
            }
        }
        catch (Exception ex) {
            Log.Exception(ex);
        }
    }

    private void RunTaskAsync(SerialTaskQueueAction task)
    {
        if (string.IsNullOrEmpty(task.Name) == false) {
            foreach (var t in _tasks) {
                if (string.IsNullOrEmpty(t.Name)) continue;
                if (t.Name == task.Name) {
                    return;
                }
            }
        }

        //await semaphore.WaitAsync();

        try {
            task.Action();
        }
        catch (Exception ex) {
            Log.Error($"action:{task.Name ?? String.Empty} exe failed: {ex.Message}");
            Log.Exception(ex);
        }

        //semaphore.Release();
    }

    public async void Stop()
    {
        if (!_isStarted) return;
        _isStarted = false;

        await Task.Run(() => {
            try {
                _isCanceled = true;
                _cancelEvent.WaitOne();
                //Log.Info($"{nameof(SerialTaskQueue)}:{_moduleName} stop", "[UTL]");
            }
            catch (Exception) {
                //Log.Error(ex.ToString());
            }
        });
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue) {
            Stop();
            disposedValue = true;
        }
    }

    ~SerialTaskQueue()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
