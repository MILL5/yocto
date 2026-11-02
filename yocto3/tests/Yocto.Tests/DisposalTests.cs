namespace Yocto.Tests;

[TestClass]
public sealed class DisposalTests
{
    [TestMethod]
    [DataRow(Lifetimes.Multiple)]
    [DataRow(Lifetimes.Singleton)]
    [DataRow(Lifetimes.Scoped)]
    [DataRow(Lifetimes.PerThread)]
    [DataRow(Lifetimes.Pooled)]
    public void Container_DisposesOwnedInstances(string lifetime)
    {
        var c = new Container();
        c.Register<Disposable>().WithLifetime(lifetime);
        var instance = c.Resolve<Disposable>();
        c.Dispose();
        c.Dispose();

        Assert.IsTrue(instance.IsDisposed);
    }

    [TestMethod]
    public void RegisterInstance_IsNeverDisposed()
    {
        var external = new Disposable();
        var c = new Container();
        c.RegisterInstance(external);
        c.Dispose();

        Assert.IsFalse(external.IsDisposed);
    }

    [TestMethod]
    public void ReplacingASingleton_DisposesThePreviousInstance()
    {
        using var c = new Container();
        var registration = c.RegisterSingleton<Disposable, Disposable>();
        var first = c.Resolve<Disposable>();
        registration.AsMultiple();

        Assert.IsTrue(first.IsDisposed);
    }

    [TestMethod]
    public void DisposingParent_DisposesChildren()
    {
        var parent = new Container();
        var child = parent.CreateChildContainer();
        var grandchild = child.CreateChildContainer();
        grandchild.Register<Disposable>().AsSingleton();
        var instance = grandchild.Resolve<Disposable>();
        parent.Dispose();

        Assert.IsTrue(instance.IsDisposed);
        Assert.ThrowsExactly<ObjectDisposedException>(() => grandchild.Resolve<Disposable>());
    }

    [TestMethod]
    public async Task DisposeAsync_DisposesAsyncOnlyServices()
    {
        var c = new Container();
        c.Register<AsyncDisposable>().AsSingleton();
        var instance = c.Resolve<AsyncDisposable>();
        await c.DisposeAsync();

        Assert.IsTrue(instance.IsDisposed);
    }

    [TestMethod]
    public void TransientsAreTrackedWeakly()
    {
        using var c = new Container();
        c.Register<Disposable>();
        var weak = CreateAndForget(c);
        GC.Collect();
        GC.WaitForPendingFinalizers();

        Assert.IsFalse(weak.TryGetTarget(out _), "The container must not keep transient instances alive.");
    }

    private static WeakReference<Disposable> CreateAndForget(IContainer c) => new(c.Resolve<Disposable>());
}
