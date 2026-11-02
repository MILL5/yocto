namespace Yocto.Tests;

[TestClass]
public sealed class RegistrationTests
{
    [TestMethod]
    public void Resolve_InjectsConstructorDependencies()
    {
        using var c = new Container();
        c.Register<IAnimal, Dog>();
        c.Register<IPerson, Person>();

        Assert.IsInstanceOfType<Dog>(c.Resolve<IPerson>().Pet);
    }

    [TestMethod]
    public void Register_ReplacesPreviousRegistration()
    {
        using var c = new Container();
        c.Register<IAnimal, Dog>();
        c.Register<IAnimal, Cat>();

        Assert.IsInstanceOfType<Cat>(c.Resolve<IAnimal>());
        Assert.HasCount(1, c.ResolveAll<IAnimal>());
    }

    [TestMethod]
    public void RegisterEnumerable_AppendsAndPreservesOrder()
    {
        using var c = new Container();
        c.RegisterEnumerable<IAnimal, Dog>();
        c.RegisterEnumerable<IAnimal, Cat>();
        c.Register<Zoo>();

        var all = c.ResolveAll<IAnimal>();
        Assert.IsInstanceOfType<Dog>(all[0]);
        Assert.IsInstanceOfType<Cat>(all[1]);
        Assert.IsInstanceOfType<Cat>(c.Resolve<IAnimal>(), "Last registration wins for single resolves.");
        Assert.HasCount(2, c.Resolve<Zoo>().Animals);
    }

    [TestMethod]
    public void Factory_ReceivesContainer()
    {
        using var c = new Container();
        c.Register<IAnimal, Cat>();
        c.Register<IPerson>(x => new Person(x.Resolve<IAnimal>()));

        Assert.IsInstanceOfType<Cat>(c.Resolve<IPerson>().Pet);
    }

    [TestMethod]
    public void Factory_ReturningWrongType_Throws()
    {
        using var c = new Container();
        c.Register(typeof(IAnimal), (_, _) => new object());

        Assert.ThrowsExactly<InvalidOperationException>(() => c.Resolve<IAnimal>());
    }

    [TestMethod]
    public void Register_RejectsAbstractAndMismatchedTypes()
    {
        using var c = new Container();
        Assert.ThrowsExactly<ArgumentException>(() => c.Register(typeof(IAnimal), typeof(IAnimal)));
        Assert.ThrowsExactly<ArgumentException>(() => c.Register(typeof(IPerson), typeof(Dog)));
    }

    [TestMethod]
    public void TryResolve_and_CanResolve_ReportMissingServices()
    {
        using var c = new Container();
        Assert.IsFalse(c.CanResolve<IAnimal>());
        Assert.IsFalse(c.TryResolve<IAnimal>(out _));
        Assert.ThrowsExactly<InvalidOperationException>(() => c.Resolve<IAnimal>());
    }

    [TestMethod]
    public void Unregister_and_Remove()
    {
        using var c = new Container();
        var registration = c.Register<IAnimal, Dog>();
        registration.Remove();
        Assert.IsFalse(c.CanResolve<IAnimal>());

        c.Register<IAnimal, Dog>();
        Assert.IsTrue(c.Unregister<IAnimal>());
        Assert.IsFalse(c.CanResolve<IAnimal>());
    }

    [TestMethod]
    public void KeyedServices()
    {
        using var c = new Container();
        c.Register<IAnimal, Dog>("dog");
        c.Register<IAnimal, Cat>("cat");

        Assert.IsInstanceOfType<Dog>(c.Resolve<IAnimal>("dog"));
        Assert.IsInstanceOfType<Cat>(c.Resolve<IAnimal>("cat"));
        Assert.IsFalse(c.CanResolve<IAnimal>());
    }

    [TestMethod]
    public void OpenGenerics()
    {
        using var c = new Container();
        c.Register(typeof(IRepository<>), typeof(Repository<>)).AsSingleton();

        var repo = c.Resolve<IRepository<Dog>>();
        Assert.IsInstanceOfType<Repository<Dog>>(repo);
        Assert.AreSame(repo, c.Resolve<IRepository<Dog>>());
        Assert.AreNotSame<object>(repo, c.Resolve<IRepository<Cat>>());
    }

    [TestMethod]
    public void Constructor_GreediestResolvableWins()
    {
        using var c = new Container();
        c.Register<Greedy>();
        Assert.AreEqual(0, c.Resolve<Greedy>().Constructor);

        c.Register<IAnimal, Dog>();
        Assert.AreEqual(1, c.Resolve<Greedy>().Constructor);
    }

    [TestMethod]
    public void CircularDependency_IsDetected()
    {
        using var c = new Container();
        c.Register<ChickenNeedsEgg>();
        c.Register<EggNeedsChicken>();

        var ex = Assert.ThrowsExactly<InvalidOperationException>(() => c.Resolve<ChickenNeedsEgg>());
        StringAssert.Contains(ex.Message, "Circular dependency");
        Assert.ThrowsExactly<AggregateException>(c.Validate);
    }

    [TestMethod]
    public void Validate_DetectsCaptiveScopedDependency()
    {
        using var c = new Container();
        c.Register<Disposable>().AsScoped();
        c.Register<Holder>().AsSingleton();

        var ex = Assert.ThrowsExactly<AggregateException>(c.Validate);
        StringAssert.Contains(ex.InnerExceptions[0].Message, "scoped");
    }

    [TestMethod]
    public void Validate_PassesForValidGraph()
    {
        using var c = new Container();
        c.Register<IAnimal, Dog>().AsSingleton();
        c.Register<IPerson, Person>();
        c.Validate();
    }

    [TestMethod]
    public void BuiltInServices_ResolveToContainer()
    {
        using var c = new Container();
        Assert.AreSame(c, c.Resolve<IContainer>());
        Assert.AreSame(c, c.Resolve<IServiceProvider>());
    }

    [TestMethod]
    public void Constructor_IsReselected_AfterDependencyUnregistered()
    {
        using var c = new Container();
        c.Register<Greedy>();
        c.Register<IAnimal, Dog>();
        c.Register<IPerson, Person>();
        Assert.AreEqual(2, c.Resolve<Greedy>().Constructor);

        c.Unregister<IPerson>();
        Assert.AreEqual(1, c.Resolve<Greedy>().Constructor);
    }

    [TestMethod]
    public void ChildFirstResolve_DoesNotBreakParent()
    {
        using var root = new Container();
        root.Register<Greedy>();
        root.Register<IAnimal, Dog>();
        using var child = root.CreateChildContainer();
        child.Register<IPerson, Person>();

        Assert.AreEqual(1, child.Resolve<Greedy>().Constructor);
        Assert.AreEqual(1, root.Resolve<Greedy>().Constructor);
    }

    [TestMethod]
    public void Validate_FromChild_DoesNotBreakParentResolve()
    {
        using var root = new Container();
        root.Register<Greedy>();
        root.Register<IAnimal, Dog>();
        using var child = root.CreateChildContainer();
        child.Register<IPerson, Person>();

        child.Validate();
        Assert.AreEqual(1, root.Resolve<Greedy>().Constructor);
    }

    [TestMethod]
    public void ChildOnlyDependency_FallsBackToResolvingContainer()
    {
        using var root = new Container();
        root.Register<Holder>();
        using var child = root.CreateChildContainer();
        child.Register<Disposable>();

        Assert.IsNotNull(child.Resolve<Holder>().Scoped);
        Assert.ThrowsExactly<InvalidOperationException>(() => root.Resolve<Holder>());
        Assert.IsNotNull(child.Resolve<Holder>().Scoped);
    }
}
