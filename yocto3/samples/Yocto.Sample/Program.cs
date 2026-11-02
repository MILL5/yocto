using Yocto;

await using var container = new Container();

container.RegisterSingleton<IClock, SystemClock>();
container.Register<IGreeter, Greeter>();
container.Register<IGreeter, LoudGreeter>("loud");
container.RegisterScoped<RequestContext, RequestContext>();
container.Validate();

Console.WriteLine(container.Resolve<IGreeter>().Greet("yocto"));
Console.WriteLine(container.Resolve<IGreeter>("loud").Greet("yocto"));

await using (var scope = container.CreateChildContainer())
{
    var request = scope.Resolve<RequestContext>();
    Console.WriteLine($"Scoped request {request.Id} is reused: {ReferenceEquals(request, scope.Resolve<RequestContext>())}");
}

internal interface IClock
{
    DateTimeOffset Now { get; }
}

internal sealed class SystemClock : IClock
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}

internal interface IGreeter
{
    string Greet(string name);
}

internal sealed class Greeter(IClock clock) : IGreeter
{
    public string Greet(string name) => $"Hello, {name}! It is {clock.Now:t}.";
}

internal sealed class LoudGreeter(IClock clock) : IGreeter
{
    public string Greet(string name) => new Greeter(clock).Greet(name).ToUpperInvariant();
}

internal sealed class RequestContext
{
    public Guid Id { get; } = Guid.NewGuid();
}
