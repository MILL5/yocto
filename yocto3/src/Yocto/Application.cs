namespace Yocto;

/// <summary>
/// An optional ambient root container for apps that cannot use constructor injection everywhere (e.g. XAML).
/// <see cref="Push"/> and <see cref="Pop"/> swap in a child container, for example per test.
/// </summary>
public static class Application
{
    private static readonly Lock s_lock = new();
    private static IContainer s_current = new Container();

    public static IContainer Current => Volatile.Read(ref s_current);

    /// <summary>Makes a new child of <see cref="Current"/> the current container and returns it.</summary>
    public static IContainer Push()
    {
        lock (s_lock)
        {
            var child = s_current.CreateChildContainer();
            Volatile.Write(ref s_current, child);
            return child;
        }
    }

    /// <summary>Disposes the current container and restores its parent. No-op at the root.</summary>
    public static IContainer Pop()
    {
        IContainer popped;
        lock (s_lock)
        {
            if (s_current.Parent is not { } parent)
                return s_current;
            popped = s_current;
            Volatile.Write(ref s_current, parent);
        }
        popped.Dispose();
        return Current;
    }
}
