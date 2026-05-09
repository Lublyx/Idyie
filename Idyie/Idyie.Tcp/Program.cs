
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using ServiceInterface.Dependency.Injection;
using ServiceInterface.Interfaces;

namespace Idyie.Tcp;

public static class Program
{

    public static async Task Main(string[] args)
    {
        
        ServiceCollection collection = new ServiceCollection();
        collection.Resolve();

        collection.AddSingleton<Server>();

        Server server = new Server(collection.BuildServiceProvider().GetRequiredService<ISIStreaming>());

        await server.StartServer();
    }
}