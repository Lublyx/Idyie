using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceInterface.Dependency.Injection;
using ServiceInterface.Interfaces;
namespace IdyieCLI;

public static class Program
{
    public static async Task Main(string[] args)
    {
        ServiceCollection collection = new ServiceCollection();
        collection.Resolve();

        App app = new App(collection.BuildServiceProvider().GetRequiredService<ISIStreaming>());
        await app.Run();
    }
}