using System.Threading;

namespace Bread.Mvc;

public delegate Task AsyncEventHandler(CancellationToken token);

public delegate Task AsyncEventHandler<T1>(T1 value1, CancellationToken token);

public delegate Task AsyncEventHandler<T1, T2>(T1 value1, T2 value2, CancellationToken token);

public delegate Task AsyncEventHandler<T1, T2, T3>(T1 value1, T2 value2, T3 value3, CancellationToken token);

public delegate Task AsyncEventHandler<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4, CancellationToken token);

public class AsyncEvent
{
    private readonly HashSet<AsyncEventHandler> _handlers;

    public AsyncEvent()
    {
        _handlers = new();
    }

    public void Add(AsyncEventHandler handler)
    {
        _handlers.Add(handler);
    }

    public void Remove(AsyncEventHandler handler)
    {
        _handlers.Remove(handler);
    }

    public async Task InvokeAsync(CancellationToken token)
    {
        foreach (var handler in _handlers) {
            if (token.IsCancellationRequested) return;
            await handler(token);
        }
    }

    public static AsyncEvent operator +(AsyncEvent? left, AsyncEventHandler right)
    {
        var result = left ?? new AsyncEvent();
        result.Add(right);
        return result;
    }

    public static AsyncEvent? operator -(AsyncEvent? left, AsyncEventHandler right)
    {
        left?.Remove(right);
        return left;
    }
}

public class AsyncEvent<T>
{
    private readonly HashSet<AsyncEventHandler<T>> _handlers;

    public AsyncEvent()
    {
        _handlers = new();
    }

    public void Add(AsyncEventHandler<T> handler)
    {
        _handlers.Add(handler);
    }

    public void Remove(AsyncEventHandler<T> handler)
    {
        _handlers.Remove(handler);
    }

    public async Task InvokeAsync(T value1, CancellationToken token)
    {
        foreach (var handler in _handlers) {
            if (token.IsCancellationRequested) return;
            await handler(value1, token);
        }
    }

    public static AsyncEvent<T> operator +(AsyncEvent<T>? left, AsyncEventHandler<T> right)
    {
        var result = left ?? new AsyncEvent<T>();
        result.Add(right);
        return result;
    }

    public static AsyncEvent<T>? operator -(AsyncEvent<T>? left, AsyncEventHandler<T> right)
    {
        left?.Remove(right);
        return left;
    }
}

public class AsyncEvent<T1, T2>
{
    private readonly HashSet<AsyncEventHandler<T1, T2>> _handlers;

    public AsyncEvent()
    {
        _handlers = new HashSet<AsyncEventHandler<T1, T2>>();
    }

    public void Add(AsyncEventHandler<T1, T2> handler)
    {
        _handlers.Add(handler);
    }

    public void Remove(AsyncEventHandler<T1, T2> handler)
    {
        _handlers.Remove(handler);
    }

    public async Task InvokeAsync(T1 value1, T2 value2, CancellationToken token)
    {
        foreach (var handler in _handlers) {
            if (token.IsCancellationRequested) return;
            await handler(value1, value2, token);
        }
    }

    public static AsyncEvent<T1, T2> operator +(AsyncEvent<T1, T2>? left, AsyncEventHandler<T1, T2> right)
    {
        var result = left ?? new AsyncEvent<T1, T2>();
        result.Add(right);
        return result;
    }

    public static AsyncEvent<T1, T2>? operator -(AsyncEvent<T1, T2>? left, AsyncEventHandler<T1, T2> right)
    {
        left?.Remove(right);
        return left;
    }
}

public class AsyncEvent<T1, T2, T3>
{
    private readonly HashSet<AsyncEventHandler<T1, T2, T3>> _handlers;

    public AsyncEvent()
    {
        _handlers = new();
    }

    public void Add(AsyncEventHandler<T1, T2, T3> handler)
    {
        _handlers.Add(handler);
    }

    public void Remove(AsyncEventHandler<T1, T2, T3> handler)
    {
        _handlers.Remove(handler);
    }

    public async Task InvokeAsync(T1 value1, T2 value2, T3 value3, CancellationToken token)
    {
        foreach (var handler in _handlers) {
            if (token.IsCancellationRequested) return;
            await handler(value1, value2, value3, token);
        }
    }

    public static AsyncEvent<T1, T2, T3> operator +(AsyncEvent<T1, T2, T3>? left, AsyncEventHandler<T1, T2, T3> right)
    {
        var result = left ?? new AsyncEvent<T1, T2, T3>();
        result.Add(right);
        return result;
    }

    public static AsyncEvent<T1, T2, T3>? operator -(AsyncEvent<T1, T2, T3>? left, AsyncEventHandler<T1, T2, T3> right)
    {
        left?.Remove(right);
        return left;
    }
}

public class AsyncEvent<T1, T2, T3, T4>
{
    private readonly HashSet<AsyncEventHandler<T1, T2, T3, T4>> _handlers;

    public AsyncEvent()
    {
        _handlers = new();
    }

    public void Add(AsyncEventHandler<T1, T2, T3, T4> handler)
    {
        _handlers.Add(handler);
    }

    public void Remove(AsyncEventHandler<T1, T2, T3, T4> handler)
    {
        _handlers.Remove(handler);
    }

    public async Task InvokeAsync(T1 value1, T2 value2, T3 value3, T4 value4, CancellationToken token)
    {
        foreach (var handler in _handlers) {
            if (token.IsCancellationRequested) return;
            await handler(value1, value2, value3, value4, token);
        }
    }

    public static AsyncEvent<T1, T2, T3, T4> operator +(AsyncEvent<T1, T2, T3, T4>? left, AsyncEventHandler<T1, T2, T3, T4> right)
    {
        var result = left ?? new AsyncEvent<T1, T2, T3, T4>();
        result.Add(right);
        return result;
    }

    public static AsyncEvent<T1, T2, T3, T4>? operator -(AsyncEvent<T1, T2, T3, T4>? left, AsyncEventHandler<T1, T2, T3, T4> right)
    {
        left?.Remove(right);
        return left;
    }
}
