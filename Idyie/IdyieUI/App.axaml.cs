using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Idyie.Application.ApplicationDependency;
using Idyie.Domain.Ports.Input;
using Microsoft.Extensions.DependencyInjection;
using ServiceInterface.Dependency.Injection;

namespace IdyieUI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        IServiceCollection collection = new ServiceCollection();
        collection.ResolveApplication();
        collection.ResolveInfrastructure();
        collection.AddSingleton<MainWindow>();

        ServiceProvider services = collection.BuildServiceProvider();

        IVideoViewerUseCase videoViewer = services.GetRequiredService<IVideoViewerUseCase>();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow(videoViewer);
        }

        base.OnFrameworkInitializationCompleted();
    }
}