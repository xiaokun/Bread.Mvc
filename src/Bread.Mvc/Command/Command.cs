namespace Bread.Mvc;

public class CommandBase
{
    public event Action<bool>? EnableChanged;
    private bool _enable = true;

    protected string CommandName = "";
    protected string ActionName = "";

    public bool Enable
    {
        get { return _enable; }
        set {
            if (_enable == value) return;
            _enable = value;
            try {
                EnableChanged?.Invoke(value);
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        }
    }

    public CommandBase(string cmdName, string action)
    {
        CommandName = cmdName;
        ActionName = action;
    }
}

public class Command : CommandBase
{
    public event Action? Event;

    public Command(string cmdName, string action) : base(cmdName, action) { }

    public void Execution(string debug = "")
    {
        try {
            if (string.IsNullOrEmpty(debug) == false) {
                Log.Info(debug, CommandName, ActionName);
            }

            Event?.Invoke();
        }
        catch (Exception ex) {
            Log.Error($"Exception:{ex.Message}\t{CommandName}\t{ActionName}");
            Log.Exception(ex);
        }
    }
}

public class Command<T> : CommandBase
{
    public event Action<T>? Event;

    public Command(string cmdName, string action) : base(cmdName, action) { }

    public void Execution(T value, string debug = "")
    {
        try {
            if (string.IsNullOrEmpty(debug) == false) {
                Log.Info(debug, CommandName, ActionName);
            }

            Event?.Invoke(value);
        }
        catch (Exception ex) {
            Log.Error($"Exception:{ex.Message}\t{CommandName}\t{ActionName}");
            Log.Exception(ex);
        }
    }
}


public class Command<T1, T2> : CommandBase
{
    public event Action<T1, T2>? Event;

    public Command(string cmdName, string action) : base(cmdName, action) { }

    public void Execution(T1 value1, T2 value2, string debug = "")
    {
        try {
            if (string.IsNullOrEmpty(debug) == false) {
                Log.Info(debug, CommandName, ActionName);
            }

            Event?.Invoke(value1, value2);
        }
        catch (Exception ex) {
            Log.Error($"Exception:{ex.Message}\t{CommandName}\t{ActionName}");
            Log.Exception(ex);
        }
    }
}

public class Command<T1, T2, T3> : CommandBase
{
    public event Action<T1, T2, T3>? Event;

    public Command(string cmdName, string action) : base(cmdName, action) { }

    public void Execution(T1 value1, T2 value2, T3 value3, string debug = "")
    {
        try {
            if (string.IsNullOrEmpty(debug) == false) {
                Log.Info(debug, CommandName, ActionName);
            }

            Event?.Invoke(value1, value2, value3);
        }
        catch (Exception ex) {
            Log.Error($"Exception:{ex.Message}\t{CommandName}\t{ActionName}");
            Log.Exception(ex);
        }
    }
}

public class Command<T1, T2, T3, T4> : CommandBase
{
    public event Action<T1, T2, T3, T4>? Event;

    public Command(string cmdName, string action) : base(cmdName, action) { }

    public void Execution(T1 value1, T2 value2, T3 value3, T4 value4, string debug = "")
    {
        if (!Enable) return;
        try {
            if (string.IsNullOrEmpty(debug) == false) {
                Log.Info($"{CommandName}\t{ActionName}\t{debug}");
            }

            Event?.Invoke(value1, value2, value3, value4);
        }
        catch (Exception ex) {
            Log.Exception(ex);
        }
    }

    public async Task ExecutionAsync(T1 value1, T2 value2, T3 value3, T4 value4, string debug = "")
    {
        await Task.Factory.StartNew(() => { Execution(value1, value2, value3, value4, debug); });
    }
}

public class Command<T1, T2, T3, T4, T5> : CommandBase
{
    public event Action<T1, T2, T3, T4, T5>? Event;

    public Command(string cmdName, string action) : base(cmdName, action) { }

    public void Execution(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, string debug = "")
    {
        if (!Enable) return;
        try {
            if (string.IsNullOrEmpty(debug) == false) {
                Log.Info($"{CommandName}\t{ActionName}\t{debug}");
            }

            Event?.Invoke(value1, value2, value3, value4, value5);
        }
        catch (Exception ex) {
            Log.Exception(ex);
        }
    }

    public async Task ExecutionAsync(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, string debug = "")
    {
        await Task.Factory.StartNew(() => { Execution(value1, value2, value3, value4, value5, debug); });
    }
}
