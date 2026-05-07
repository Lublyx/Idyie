using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceInterface.Dependency.Injection;
using ServiceInterface.Interfaces;
using VisioForge.DotNet.VideoCapture;
namespace IdyieCLI;

public static class Program
{
    public static void Main(string[] args)
    {
        ServiceCollection collection = new ServiceCollection();
        collection.Resolve();

        App app = new App(collection.BuildServiceProvider().GetRequiredService<IStreaming>());
        app.Run();
    }
}