using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Yocto.Tests;

[TestClass]
public sealed class ContainerTests
{
    [TestMethod]
    public void ChildContainer_FallsBackToParent_and_CanOverride()
    {
        using var parent = new Container();
        parent.Register<IAnimal, Dog>();
        using var child = parent.CreateChildContainer();

        Assert.IsInstanceOfType<Dog>(child.Resolve<IAnimal>());
        child.Register<IAnimal, Cat>();
        Assert.IsInstanceOfType<Cat>(child.Resolve<IAnimal>());
        Assert.IsInstanceOfType<Dog>(parent.Resolve<IAnimal>());
        Assert.AreSame(parent, child.Parent);
    }

    [TestMethod]
    public void ContainerBuilder_BuildsOnce()
    {
        using var builder = new ContainerBuilder { ValidateOnBuild = true };
        builder.Register<IAnimal, Dog>();
        builder.RegisterModule<PersonModule>();
        using var container = builder.Build();

        Assert.IsInstanceOfType<Person>(container.Resolve<IPerson>());
        Assert.ThrowsExactly<InvalidOperationException>(() => builder.Build());
        Assert.ThrowsExactly<InvalidOperationException>(() => builder.Register<IAnimal, Cat>());
    }

    [TestMethod]
    public void ContainerBuilder_WithParent_CreatesChild()
    {
        using var parent = new Container();
        parent.Register<IAnimal, Dog>();
        using var builder = new ContainerBuilder(parent);
        builder.Register<IPerson, Person>();
        using var container = builder.Build();

        Assert.AreSame(parent, container.Parent);
        Assert.IsInstanceOfType<Dog>(container.Resolve<IPerson>().Pet);
    }

    [TestMethod]
    public void RegisterAssembly_AppliesConventionAndModules()
    {
        using var c = new Container();
        var applied = c.RegisterAssembly(Assembly.GetExecutingAssembly());

        Assert.AreEqual(2, applied);
        Assert.IsInstanceOfType<Dog>(c.Resolve<IPerson>().Pet);
    }

    [TestMethod]
    [DoNotParallelize]
    public void Application_PushAndPop()
    {
        var root = Application.Current;
        var pushed = Application.Push();
        pushed.Register<IAnimal, Dog>();

        Assert.AreSame(pushed, Application.Current);
        Assert.AreSame(root, pushed.Parent);
        Assert.AreSame(root, Application.Pop());
        Assert.IsFalse(Application.Current.CanResolve<IAnimal>());
        Assert.AreSame(root, Application.Pop(), "Popping the root is a no-op.");
    }

    [TestMethod]
    public async Task GenericHost_UsesYoctoAsServiceProvider()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddSingleton<IAnimal, Dog>();
        builder.ConfigureContainer(new YoctoServiceProviderFactory(validateOnBuild: true), c => c.Register<IPerson, Person>());
        using var host = builder.Build();
        await host.StartAsync();

        Assert.IsInstanceOfType<Container>(host.Services);
        Assert.IsInstanceOfType<Dog>(host.Services.GetRequiredService<IPerson>().Pet);
        await host.StopAsync();
    }
}
