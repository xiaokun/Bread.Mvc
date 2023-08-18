using System.Threading;

namespace Bread.Mvc;

public class AsyncCommandBase
{
    protected string CommandName = "";
    protected string ActionName = "";

    public AsyncCommandBase(string cmdName, string action)
    {
        CommandName = cmdName;
        ActionName = action;
    }
}

public class AsyncCommand : AsyncCommandBase
{
    public event Action? Event;

    public AsyncEvent? AsyncEvent = null!;

    public AsyncCommand(string cmdName, string action) : base(cmdName, action) { }

    public async Task ExecutionAsync(CancellationToken token = default, string debug = "")
    {
        try {
            if (string.IsNullOrEmpty(debug) == false) {
                Log.Info(debug, CommandName, ActionName);
            }

            Event?.Invoke();
            if (token.IsCancellationRequested) return;
            if (AsyncEvent != null)
                await AsyncEvent.InvokeAsync(token);
        }
        catch (Exception ex) {
            Log.Error($"Exception:{ex.Message}\t{CommandName}\t{ActionName}");
            Log.Exception(ex);
        }
    }
}

public class AsyncCommand<T> : AsyncCommandBase
{
    public event Action<T>? Event;

    public AsyncEvent<T>? AsyncEvent = null!;

    public AsyncCommand(string cmdName, string action) : base(cmdName, action) { }


    public async Task ExecutionAsync(T value, CancellationToken token = default, string debug = "")
    {
        try {
            if (string.IsNullOrEmpty(debug) == false) {
                Log.Info(debug, CommandName, ActionName);
            }

            Event?.Invoke(value);
            if (token.IsCancellationRequested) return;
            if (AsyncEvent != null)
                await AsyncEvent.InvokeAsync(value, token);
        }
        catch (Exception ex) {
            Log.Error($"Exception:{ex.Message}\t{CommandName}\t{ActionName}");
            Log.Exception(ex);
        }
    }
}


public class AsyncCommand<T1, T2> : AsyncCommandBase
{
    public event Action<T1, T2>? Event;

    public AsyncEvent<T1, T2>? AsyncEvent = null!;

    public AsyncCommand(string cmdName, string action) : base(cmdName, action) { }

    public async Task ExecutionAsync(T1 value1, T2 value2, CancellationToken token = default, string debug = "")
    {
        try {
            if (string.IsNullOrEmpty(debug) == false) {
                Log.Info(debug, CommandName, ActionName);
            }

            Event?.Invoke(value1, value2);
            if (token.IsCancellationRequested) return;
            if (AsyncEvent != null)
                await AsyncEvent.InvokeAsync(value1, value2, token);
        }
        catch (Exception ex) {
            Log.Error($"Exception:{ex.Message}\t{CommandName}\t{ActionName}");
            Log.Exception(ex);
        }
    }
}

public class AsyncCommand<T1, T2, T3> : AsyncCommandBase
{
    public event Action<T1, T2, T3>? Event;

    public AsyncEvent<T1, T2, T3>? AsyncEvent = null!;

    public AsyncCommand(string cmdName, string action) : base(cmdName, action) { }

    public async Task ExecutionAsync(T1 value1, T2 value2, T3 value3, CancellationToken token = default, string debug = "")
    {
        try {
            if (string.IsNullOrEmpty(debug) == false) {
                Log.Info(debug, CommandName, ActionName);
            }

            Event?.Invoke(value1, value2, value3);
            if (token.IsCancellationRequested) return;
            if (AsyncEvent != null)
                await AsyncEvent.InvokeAsync(value1, value2, value3, token);
        }
        catch (Exception ex) {
            Log.Error($"Exception:{ex.Message}\t{CommandName}\t{ActionName}");
            Log.Exception(ex);
        }
    }
}

public class AsyncCommand<T1, T2, T3, T4> : AsyncCommandBase
{
    public event Action<T1, T2, T3, T4>? Event;

    public AsyncEvent<T1, T2, T3, T4>? AsyncEvent = null!;

    public AsyncCommand(string cmdName, string action) : base(cmdName, action) { }


    public async Task ExecutionAsync(T1 value1, T2 value2, T3 value3, T4 value4, CancellationToken token = default, string debug = "")
    {
        try {
            if (string.IsNullOrEmpty(debug) == false) {
                Log.Info(debug, CommandName, ActionName);
            }

            Event?.Invoke(value1, value2, value3, value4);
            if (token.IsCancellationRequested) return;
            if (AsyncEvent != null)
                await AsyncEvent.InvokeAsync(value1, value2, value3, value4, token);
        }
        catch (Exception ex) {
            Log.Error($"Exception:{ex.Message}\t{CommandName}\t{ActionName}");
            Log.Exception(ex);
        }
    }
}

