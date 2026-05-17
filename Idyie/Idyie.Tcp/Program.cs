using Idyie.Application.ApplicationDependency;
using Idyie.Domain.Ports.Input;
using Microsoft.Extensions.DependencyInjection;
using ServiceInterface.Dependency.Injection;

namespace Idyie.Tcp;

public static class Program
{

    public static async Task Main(string[] args)
    {
        
        ServiceCollection collection = new ServiceCollection();
        collection.ResolveApplication();
        collection.ResolveInfrastructure();

        collection.AddSingleton<Server>();

        Server server = new Server(collection.BuildServiceProvider().GetRequiredService<IServerRedirectingUseCase>());

        await server.StartServer();
    }
}