using System.Windows.Input;

namespace Bread.Mvc.Avalonia;

public class DelegateCommand : ICommand
{
    Func<object?, bool> _canExecute;
    Action<object?> _executeAction;

    bool canExecuteCache;

    public DelegateCommand(Action<object?> executeAction, Func<object?, bool> canExecute)
    {
        this._executeAction = executeAction;
        this._canExecute = canExecute;
    }

    #region ICommand Members

    public bool CanExecute(object? parameter)
    {
        bool temp = _canExecute(parameter);

        if (canExecuteCache != temp) {
            canExecuteCache = temp;
            if (CanExecuteChanged != null) {
                CanExecuteChanged(this, new EventArgs());
            }
        }

        return canExecuteCache;
    }

    public event EventHandler? CanExecuteChanged;

    public void Execute(object? parameter)
    {
        _executeAction(parameter);
    }

    #endregion
}
