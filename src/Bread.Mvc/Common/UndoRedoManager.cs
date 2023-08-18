namespace Bread.Mvc;

public static class UndoCommands
{
    public static Command BeginGroup { get; } = new(nameof(UndoCommands), nameof(BeginGroup));

    //undo and redo control
    public static Command Undo { get; } = new(nameof(UndoCommands), nameof(Undo));

    public static Command Redo { get; } = new(nameof(UndoCommands), nameof(Redo));

    public static Command EndGroup { get; } = new(nameof(UndoCommands), nameof(BeginGroup));

    public static Command Clear { get; } = new(nameof(UndoCommands), nameof(Clear));
}

#region UndoRedoOperation 

/// <summary>
/// 封装 undo 和 redo 的操作
/// </summary>
public abstract class UndoRedoOperation
{
    public abstract bool Run();
}

#region Functions

public class UndoRedoFuction : UndoRedoOperation
{
    readonly Func<bool> _action;

    public UndoRedoFuction(Func<bool> action)
    {
        _action = action;
    }

    public override bool Run()
    {
        return _action();
    }
}

public class UndoRedoFuction<T> : UndoRedoOperation
{
    readonly Func<T, bool> _func;
    readonly T _arg;

    public UndoRedoFuction(Func<T, bool> func, T arg)
    {
        _arg = arg;
        _func = func;
    }

    public override bool Run()
    {
        return _func(_arg);
    }
}

public class UndoRedoFuction<T1, T2> : UndoRedoOperation
{
    readonly Func<T1, T2, bool> _func;
    readonly T1 _arg1;
    readonly T2 _arg2;

    public UndoRedoFuction(Func<T1, T2, bool> func, T1 arg1, T2 arg2)
    {
        _func = func;
        _arg1 = arg1;
        _arg2 = arg2;
    }

    public override bool Run()
    {
        return _func(_arg1, _arg2);
    }
}

public class UndoRedoFuction<T1, T2, T3> : UndoRedoOperation
{
    readonly Func<T1, T2, T3, bool> _func;
    readonly T1 _arg1;
    readonly T2 _arg2;
    readonly T3 _arg3;

    public UndoRedoFuction(Func<T1, T2, T3, bool> func, T1 arg1, T2 arg2, T3 arg3)
    {
        _func = func;
        _arg1 = arg1;
        _arg2 = arg2;
        _arg3 = arg3;
    }

    public override bool Run()
    {
        return _func(_arg1, _arg2, _arg3);
    }
}

public class UndoRedoFuction<T1, T2, T3, T4> : UndoRedoOperation
{
    readonly Func<T1, T2, T3, T4, bool> _func;
    readonly T1 _arg1;
    readonly T2 _arg2;
    readonly T3 _arg3;
    readonly T4 _arg4;

    public UndoRedoFuction(Func<T1, T2, T3, T4, bool> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        _func = func;
        _arg1 = arg1;
        _arg2 = arg2;
        _arg3 = arg3;
        _arg4 = arg4;
    }

    public override bool Run()
    {
        return _func(_arg1, _arg2, _arg3, _arg4);
    }
}

#endregion

#region Actions

public class UndoRedoAction : UndoRedoOperation
{
    readonly Action _action;

    public UndoRedoAction(Action action)
    {
        _action = action;
    }

    public override bool Run()
    {
        _action();
        return true;
    }
}

public class UndoRedoAction<T> : UndoRedoOperation
{
    readonly Action<T> _action;
    readonly T _arg;

    public UndoRedoAction(Action<T> action, T arg)
    {
        _arg = arg;
        _action = action;
    }

    public override bool Run()
    {
        _action(_arg);
        return true;
    }
}

public class UndoRedoAction<T1, T2> : UndoRedoOperation
{
    readonly Action<T1, T2> _action;
    readonly T1 _arg1;
    readonly T2 _arg2;

    public UndoRedoAction(Action<T1, T2> action, T1 arg1, T2 arg2)
    {
        _action = action;
        _arg1 = arg1;
        _arg2 = arg2;
    }

    public override bool Run()
    {
        _action(_arg1, _arg2);
        return true;
    }
}

public class UndoRedoAction<T1, T2, T3> : UndoRedoOperation
{
    readonly Action<T1, T2, T3> _action;
    readonly T1 _arg1;
    readonly T2 _arg2;
    readonly T3 _arg3;

    public UndoRedoAction(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
    {
        _action = action;
        _arg1 = arg1;
        _arg2 = arg2;
        _arg3 = arg3;
    }

    public override bool Run()
    {
        _action(_arg1, _arg2, _arg3);
        return true;
    }
}

public class UndoRedoAction<T1, T2, T3, T4> : UndoRedoOperation
{
    readonly Action<T1, T2, T3, T4> _action;
    readonly T1 _arg1;
    readonly T2 _arg2;
    readonly T3 _arg3;
    readonly T4 _arg4;

    public UndoRedoAction(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        _action = action;
        _arg1 = arg1;
        _arg2 = arg2;
        _arg3 = arg3;
        _arg4 = arg4;
    }

    public override bool Run()
    {
        _action(_arg1, _arg2, _arg3, _arg4);
        return true;
    }
}

#endregion



public static class Undo
{
    public static UndoRedoAction New(Action action)
    {
        return new UndoRedoAction(action);
    }

    public static UndoRedoAction<T1> New<T1>(Action<T1> action, T1 arg1)
    {
        return new UndoRedoAction<T1>(action, arg1);
    }

    public static UndoRedoAction<T1, T2> New<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
    {
        return new UndoRedoAction<T1, T2>(action, arg1, arg2);
    }

    public static UndoRedoAction<T1, T2, T3> New<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
    {
        return new UndoRedoAction<T1, T2, T3>(action, arg1, arg2, arg3);
    }

    public static UndoRedoAction<T1, T2, T3, T4> New<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        return new UndoRedoAction<T1, T2, T3, T4>(action, arg1, arg2, arg3, arg4);
    }


    public static UndoRedoFuction New(Func<bool> func)
    {
        return new UndoRedoFuction(func);
    }

    public static UndoRedoFuction<T1> New<T1>(Func<T1, bool> func, T1 arg1)
    {
        return new UndoRedoFuction<T1>(func, arg1);
    }

    public static UndoRedoFuction<T1, T2> New<T1, T2>(Func<T1, T2, bool> func, T1 arg1, T2 arg2)
    {
        return new UndoRedoFuction<T1, T2>(func, arg1, arg2);
    }

    public static UndoRedoFuction<T1, T2, T3> New<T1, T2, T3>(Func<T1, T2, T3, bool> func, T1 arg1, T2 arg2, T3 arg3)
    {
        return new UndoRedoFuction<T1, T2, T3>(func, arg1, arg2, arg3);
    }

    public static UndoRedoFuction<T1, T2, T3, T4> New<T1, T2, T3, T4>(Func<T1, T2, T3, T4, bool> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        return new UndoRedoFuction<T1, T2, T3, T4>(func, arg1, arg2, arg3, arg4);
    }
}


public static class Redo
{
    public static UndoRedoAction New(Action action)
    {
        return new UndoRedoAction(action);
    }

    public static UndoRedoAction<T1> New<T1>(Action<T1> action, T1 arg1)
    {
        return new UndoRedoAction<T1>(action, arg1);
    }

    public static UndoRedoAction<T1, T2> New<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
    {
        return new UndoRedoAction<T1, T2>(action, arg1, arg2);
    }

    public static UndoRedoAction<T1, T2, T3> New<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
    {
        return new UndoRedoAction<T1, T2, T3>(action, arg1, arg2, arg3);
    }

    public static UndoRedoAction<T1, T2, T3, T4> New<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        return new UndoRedoAction<T1, T2, T3, T4>(action, arg1, arg2, arg3, arg4);
    }


    public static UndoRedoFuction New(Func<bool> func)
    {
        return new UndoRedoFuction(func);
    }

    public static UndoRedoFuction<T1> New<T1>(Func<T1, bool> func, T1 arg1)
    {
        return new UndoRedoFuction<T1>(func, arg1);
    }

    public static UndoRedoFuction<T1, T2> New<T1, T2>(Func<T1, T2, bool> func, T1 arg1, T2 arg2)
    {
        return new UndoRedoFuction<T1, T2>(func, arg1, arg2);
    }

    public static UndoRedoFuction<T1, T2, T3> New<T1, T2, T3>(Func<T1, T2, T3, bool> func, T1 arg1, T2 arg2, T3 arg3)
    {
        return new UndoRedoFuction<T1, T2, T3>(func, arg1, arg2, arg3);
    }

    public static UndoRedoFuction<T1, T2, T3, T4> New<T1, T2, T3, T4>(Func<T1, T2, T3, T4, bool> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        return new UndoRedoFuction<T1, T2, T3, T4>(func, arg1, arg2, arg3, arg4);
    }
}
#endregion

internal class OperationPair
{
    public UndoRedoOperation Undo { get; set; }

    public UndoRedoOperation Redo { get; set; }

    public long GroupId { get; set; } = 0;

    public OperationPair(UndoRedoOperation undo, UndoRedoOperation redo)
    {
        Undo = undo;
        Redo = redo;
    }
}


public class UndoRedoMolde : Model
{
    Stack<OperationPair> UndoList = new();
    Stack<OperationPair> RedoList = new();

    public bool CanUndo { get { return UndoList.Count > 0; } }

    public bool CanRedo { get { return RedoList.Count > 0; } }

    internal void Undo()
    {
        if (UndoList.Count == 0) return;
        var op = UndoList.Pop();
        var success = op.Undo.Run();
        if (success == false) {
            Log.Error("undo action exec fail.");
        }
        RedoList.Push(op);

        if (op.GroupId != 0) {
            success = true;
            while (UndoList.Count > 0) {
                var header = UndoList.Peek();
                if (header.GroupId == 0) break;
                if (header.GroupId != op.GroupId) break;
                success |= header.Undo.Run();
                RedoList.Push(header);
                UndoList.Pop();
            }
            if (success == false) {
                Log.Error("undo actions exec fail.");
            }
        }

        if (UndoList.Count == 0) {
            UndoCommands.Undo.Enable = false;
        }
        UndoCommands.Redo.Enable = true;
    }

    internal void Redo()
    {
        if (RedoList.Count == 0) return;

        var op = RedoList.Pop();
        var success = op.Redo.Run();
        if (success == false) {
            Log.Error("redo action exec fail.");
        }
        UndoList.Push(op);

        if (op.GroupId != 0) {
            success = true;
            while (RedoList.Count > 0) {
                var header = RedoList.Peek();
                if (header.GroupId == 0) break;
                if (header.GroupId != op.GroupId) break;
                success |= header.Redo.Run();
                UndoList.Push(header);
                RedoList.Pop();
            }
            if (success == false) {
                Log.Error("redo actions exec fail.");
            }
        }

        if (RedoList.Count == 0) {
            UndoCommands.Redo.Enable = false;
        }
        UndoCommands.Undo.Enable = true;
    }

    bool _group = false;
    long _groupId = 0;

    internal void BeginGroup()
    {
        if (_group) return;
        _group = true;
        _groupId++;
        //Log.Info($"group id : {_groupId}");
    }

    internal void EndGroup()
    {
        _group = false;
    }

    public void AddAction(UndoRedoOperation undo, UndoRedoOperation redo)
    {
        var pair = new OperationPair(undo, redo);
        if (_group) {
            pair.GroupId = _groupId;
            //Log.Info($"add actino id : {_groupId}");
        }
        UndoList.Push(pair);
        RedoList.Clear();
        UndoCommands.Redo.Enable = false;
        UndoCommands.Undo.Enable = true;
    }

    public override void Clear()
    {
        UndoList.Clear();
        RedoList.Clear();
    }
}


public class UndoRedoController : Controller
{
    UndoRedoMolde _model;

    public UndoRedoController(UndoRedoMolde model)
    {
        _model = model;

        UndoCommands.Undo.Enable = false;
        UndoCommands.Redo.Enable = false;
        UndoCommands.Undo.Event += Undo_Event;
        UndoCommands.Redo.Event += Redo_Event;
        UndoCommands.Clear.Event += Clear_Event;
        UndoCommands.BeginGroup.Event += BeginGroup_Event;
        UndoCommands.EndGroup.Event += EndGroup_Event;
    }


    private void Redo_Event()
    {
        _model.Redo();
    }

    private void Undo_Event()
    {
        _model.Undo();
    }

    private void Clear_Event()
    {
        _model.Clear();
    }

    private void BeginGroup_Event()
    {
        _model.BeginGroup();
    }

    private void EndGroup_Event()
    {
        _model.EndGroup();
    }
}
