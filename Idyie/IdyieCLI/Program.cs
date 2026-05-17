using Idyie.Application.ApplicationDependency;
using Idyie.Domain.Ports.Input;
using Microsoft.Extensions.DependencyInjection;
using ServiceInterface.Dependency.Injection;
namespace IdyieCLI;

public static class Program
{
    public static async Task Main(string[] args)
    {
        ServiceCollection collection = new ServiceCollection();
        collection.ResolveApplication();
        collection.ResolveInfrastructure();

        App app = new App(collection.BuildServiceProvider().GetRequiredService<IStreamVideoUseCase>());
        await app.Run();
    }
}