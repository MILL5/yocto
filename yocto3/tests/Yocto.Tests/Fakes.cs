namespace Yocto.Tests;

public interface IAnimal;
public class Dog : IAnimal;
public class Cat : IAnimal;

public interface IPerson
{
    IAnimal Pet { get; }
}

public class Person(IAnimal pet) : IPerson
{
    public IAnimal Pet { get; } = pet;
}

public interface IRepository<T>;
public class Repository<T> : IRepository<T>;
public class Unrelated<T>;

public sealed class Disposable : IDisposable
{
    public bool IsDisposed { get; private set; }
    public void Dispose() => IsDisposed = true;
}

public sealed class AsyncDisposable : IAsyncDisposable
{
    public bool IsDisposed { get; private set; }
    public ValueTask DisposeAsync()
    {
        IsDisposed = true;
        return ValueTask.CompletedTask;
    }
}

public class ChickenNeedsEgg(EggNeedsChicken egg)
{
    public EggNeedsChicken Egg { get; } = egg;
}

public class EggNeedsChicken(ChickenNeedsEgg chicken)
{
    public ChickenNeedsEgg Chicken { get; } = chicken;
}

public class Greedy
{
    public Greedy() => Constructor = 0;
    public Greedy(IAnimal animal) => Constructor = 1;
    public Greedy(IAnimal animal, IPerson person) => Constructor = 2;
    public int Constructor { get; }
}

public class Zoo(IEnumerable<IAnimal> animals)
{
    public IReadOnlyList<IAnimal> Animals { get; } = [.. animals];
}

public class Holder(Disposable scoped)
{
    public Disposable Scoped { get; } = scoped;
}

/// <summary>Picked up by <see cref="AutoRegistration.RegisterAssembly"/> by naming convention.</summary>
public static class AssemblyRegistration
{
    public static void Initialize(IContainer container) => container.Register<IAnimal, Dog>();
}

public sealed class PersonModule : IModule
{
    public void Register(IRegistrar registrar) => registrar.Register<IPerson, Person>();
}
