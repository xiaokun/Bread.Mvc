using System.Diagnostics;
using System.Windows.Input;

namespace Bread.Mvc.Avalonia;

public class RelayCommand : ICommand
{
    readonly Action _execute;
    readonly Func<bool>? _canExecute;

    public RelayCommand(Action execute) : this(execute, null)
    {
    }

    public RelayCommand(Action execute, Func<bool>? canExecute)
    {
        if (execute == null)
            throw new ArgumentNullException("execute");

        _execute = execute;
        _canExecute = canExecute;
    }

    [DebuggerStepThrough]
    public bool CanExecute(object? parameter)
    {
        return _canExecute == null ? true : _canExecute();
    }
    public event EventHandler? CanExecuteChanged;

    public void Execute(object? parameter)
    {
        _execute();
    }
}

public class RelayCommand<T> : ICommand
{
    readonly Action<T?> _execute;
    readonly Predicate<T>? _canExecute = null;

    public event EventHandler? CanExecuteChanged;

    public RelayCommand(Action<T?> execute)
        : this(execute, null)
    {
    }

    public RelayCommand(Action<T?> execute, Predicate<T>? canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    [DebuggerStepThrough]
    public bool CanExecute(object? parameter)
    {
        if (parameter == null) return false;
        return _canExecute == null ? true : _canExecute((T)parameter);
    }

    public void Execute(object? parameter)
    {
        _execute((T?)parameter);
    }
}
