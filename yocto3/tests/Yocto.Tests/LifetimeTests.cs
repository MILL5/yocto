namespace Yocto.Tests;

[TestClass]
public sealed class LifetimeTests
{
    [TestMethod]
    public void Multiple_CreatesNewInstances()
    {
        using var c = new Container();
        c.Register<IAnimal, Dog>();
        Assert.AreNotSame(c.Resolve<IAnimal>(), c.Resolve<IAnimal>());
    }

    [TestMethod]
    public void Singleton_SharesOneInstance()
    {
        using var c = new Container();
        c.RegisterSingleton<IAnimal, Dog>();
        Assert.AreSame(c.Resolve<IAnimal>(), c.Resolve<IAnimal>());
    }

    [TestMethod]
    public void Singleton_EagerLoadCreatesImmediately()
    {
        using var c = new Container();
        var created = 0;
        c.Register<IAnimal>(() => { created++; return new Dog(); }).AsSingleton(eagerLoad: true);
        Assert.AreEqual(1, created);
        c.Resolve<IAnimal>();
        Assert.AreEqual(1, created);
    }

    [TestMethod]
    public void Scoped_OneInstancePerContainer()
    {
        using var c = new Container();
        c.RegisterScoped<IAnimal, Dog>();
        using var child = c.CreateChildContainer();

        Assert.AreSame(child.Resolve<IAnimal>(), child.Resolve<IAnimal>());
        Assert.AreNotSame(c.Resolve<IAnimal>(), child.Resolve<IAnimal>());
    }

    [TestMethod]
    public void PerThread_OneInstancePerThread()
    {
        using var c = new Container();
        c.RegisterPerThread<IAnimal, Dog>();
        var main = c.Resolve<IAnimal>();
        IAnimal? other = null;
        var thread = new Thread(() => other = c.Resolve<IAnimal>());
        thread.Start();
        thread.Join();

        Assert.AreSame(main, c.Resolve<IAnimal>());
        Assert.IsNotNull(other);
        Assert.AreNotSame(main, other);
    }

    [TestMethod]
    public void Pooled_RoundRobinsOverPool()
    {
        using var c = new Container();
        c.RegisterPooled<IAnimal, Dog>(poolSize: 4);
        var first = Enumerable.Range(0, 4).Select(_ => c.Resolve<IAnimal>()).ToArray();
        var second = Enumerable.Range(0, 4).Select(_ => c.Resolve<IAnimal>()).ToArray();

        Assert.HasCount(4, first.Distinct());
        CollectionAssert.AreEqual(first, second);
    }

    [TestMethod]
    public void Pooled_IsThreadSafe()
    {
        using var c = new Container();
        c.RegisterPooled<IAnimal, Dog>(poolSize: 4);
        var seen = new System.Collections.Concurrent.ConcurrentBag<IAnimal>();
        Parallel.For(0, 10_000, _ => seen.Add(c.Resolve<IAnimal>()));
        Assert.HasCount(4, seen.Distinct());
    }

    [TestMethod]
    public void Pooled_RejectsTinyPools()
    {
        using var c = new Container();
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => c.RegisterPooled<IAnimal, Dog>(poolSize: 1));
    }

    [TestMethod]
    public void CustomLifetime_CanBeRegistered()
    {
        Lifetimes.Register("AlwaysTheSameDog", new SameDogLifetime());
        using var c = new Container();
        c.Register<IAnimal, Cat>().WithLifetime("AlwaysTheSameDog");

        Assert.AreSame(SameDogLifetime.Dog, c.Resolve<IAnimal>());
        Assert.ThrowsExactly<NotSupportedException>(() => c.Register<IAnimal, Cat>().WithLifetime("Nope"));
    }

    private sealed class SameDogLifetime : ILifetimeFactory, IInstanceFactory
    {
        public static readonly Dog Dog = new();
        public IInstanceFactory CreateInstanceFactory(LifetimeContext context, IReadOnlyList<object?> options) => this;
        public object GetInstance(IContainer scope) => Dog;
        public void Dispose()
        {
        }
    }
}
