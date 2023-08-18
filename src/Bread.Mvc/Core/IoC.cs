using ZeroIoC;

namespace Bread.Mvc;

public interface IHiddenInIoC { }

public class Controller : IHiddenInIoC { }

public partial class IoCResolver : ZeroIoCContainer
{
    protected override void Bootstrap(IZeroIoCContainerBootstrapper builder)
    {
        builder.AddSingleton<UndoRedoMolde>();
        builder.AddSingleton<UndoRedoController>();
    }

    public void End()
    {
        foreach (var type in Resolvers.Keys) {
            if (type.IsSubclassOf(typeof(Controller))) {
                var value = Resolvers[type];
                _ = value.Resolve(this);
            }
        }

        foreach (var type in ScopedResolvers.Keys) {
            if (type.IsSubclassOf(typeof(Controller))) {
                var value = ScopedResolvers[type];
                _ = value.Resolve(this);
            }
        }
    }
}

public static class IoC
{
    public static IUIDispatcher? MainThread { get; private set; } = null;

    private static IoCResolver Resolver = new IoCResolver();

    public static void Init(params ZeroIoCContainer[] containers)
    {
        foreach (var c in containers) {
            Resolver.Merge(c);
        }
        Resolver.End();

        MainThread = Resolver.Resolve<IUIDispatcher>();
    }

    public static T Get<T>() where T : class
    {
        var o = Resolver.Resolve<T>() ?? throw new TypeAccessException($"{nameof(T)} not found in IoC container");
        if (o is IHiddenInIoC) throw new InvalidProgramException("Hidden object can't resolve by IoC container");
        return o;
    }

    public static IoCResolver Get<T>(out T model) where T : class
    {
        var t = Resolver.Resolve<T>() ?? throw new TypeAccessException($"{nameof(T)} not found in IoC container");
        if (t is IHiddenInIoC) throw new InvalidProgramException("Hidden object can't resolve by IoC container");
        model = t;
        return Resolver;
    }

    public static IoCResolver Get<T1, T2>(out T1 m1, out T2 m2) where T1 : class where T2 : class
    {
        var t1 = Resolver.Resolve<T1>() ?? throw new TypeAccessException($"{nameof(T1)} not found in IoC container");
        if (t1 is IHiddenInIoC) throw new InvalidProgramException("Hidden object can't resolve by IoC container");
        m1 = t1;

        var t2 = Resolver.Resolve<T2>() ?? throw new TypeAccessException($"{nameof(T2)} not found in IoC container");
        if (t2 is IHiddenInIoC) throw new InvalidProgramException("Hidden object can't resolve by IoC container");
        m2 = t2;

        return Resolver;
    }

    public static IoCResolver Get<T1, T2, T3>(out T1 m1, out T2 m2, out T3 m3) where T1 : class where T2 : class where T3 : class
    {
        var t1 = Resolver.Resolve<T1>() ?? throw new TypeAccessException($"{nameof(T1)} not found in IoC container");
        if (t1 is IHiddenInIoC) throw new InvalidProgramException("Hidden object can't resolve by IoC container");
        m1 = t1;

        var t2 = Resolver.Resolve<T2>() ?? throw new TypeAccessException($"{nameof(T2)} not found in IoC container");
        if (t2 is IHiddenInIoC) throw new InvalidProgramException("Hidden object can't resolve by IoC container");
        m2 = t2;

        var t3 = Resolver.Resolve<T3>() ?? throw new TypeAccessException($"{nameof(T3)} not found in IoC container");
        if (t3 is IHiddenInIoC) throw new InvalidProgramException("Hidden object can't resolve by IoC container");
        m3 = t3;

        return Resolver;
    }

    public static IoCResolver Get<T1, T2, T3, T4>(out T1 m1, out T2 m2, out T3 m3, out T4 m4)
        where T1 : class where T2 : class where T3 : class where T4 : class
    {
        var t1 = Resolver.Resolve<T1>() ?? throw new TypeAccessException($"{nameof(T1)} not found in IoC container");
        if (t1 is IHiddenInIoC) throw new InvalidProgramException("Hidden object can't resolve by IoC container");
        m1 = t1;

        var t2 = Resolver.Resolve<T2>() ?? throw new TypeAccessException($"{nameof(T2)} not found in IoC container");
        if (t2 is IHiddenInIoC) throw new InvalidProgramException("Hidden object can't resolve by IoC container");
        m2 = t2;

        var t3 = Resolver.Resolve<T3>() ?? throw new TypeAccessException($"{nameof(T3)} not found in IoC container");
        if (t3 is IHiddenInIoC) throw new InvalidProgramException("Hidden object can't resolve by IoC container");
        m3 = t3;

        var t4 = Resolver.Resolve<T4>() ?? throw new TypeAccessException($"{nameof(T4)} not found in IoC container");
        if (t4 is IHiddenInIoC) throw new InvalidProgramException("Hidden object can't resolve by IoC container");
        m4 = t4;

        return Resolver;
    }

    public static IoCResolver Get<T1, T2, T3, T4, T5>(out T1 m1, out T2 m2, out T3 m3, out T4 m4, out T5 m5)
       where T1 : class where T2 : class where T3 : class where T4 : class where T5 : class
    {
        var t1 = Resolver.Resolve<T1>() ?? throw new TypeAccessException($"{nameof(T1)} not found in IoC container");
        if (t1 is IHiddenInIoC) throw new InvalidProgramException("Hidden object can't resolve by IoC container");
        m1 = t1;

        var t2 = Resolver.Resolve<T2>() ?? throw new TypeAccessException($"{nameof(T2)} not found in IoC container");
        if (t2 is IHiddenInIoC) throw new InvalidProgramException("Hidden object can't resolve by IoC container");
        m2 = t2;

        var t3 = Resolver.Resolve<T3>() ?? throw new TypeAccessException($"{nameof(T3)} not found in IoC container");
        if (t3 is IHiddenInIoC) throw new InvalidProgramException("Hidden object can't resolve by IoC container");
        m3 = t3;

        var t4 = Resolver.Resolve<T4>() ?? throw new TypeAccessException($"{nameof(T4)} not found in IoC container");
        if (t4 is IHiddenInIoC) throw new InvalidProgramException("Hidden object can't resolve by IoC container");
        m4 = t4;

        var t5 = Resolver.Resolve<T5>() ?? throw new TypeAccessException($"{nameof(T5)} not found in IoC container");
        if (t5 is IHiddenInIoC) throw new InvalidProgramException("Hidden object can't resolve by IoC container");
        m5 = t5;

        return Resolver;
    }

    public static void Dispose()
    {
        Resolver?.Dispose();
    }
}
