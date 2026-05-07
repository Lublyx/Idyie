using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using ServiceInterface.Dependency.Injection;
using ServiceInterface.Interfaces;

namespace IdyieUI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        ServiceCollection collection = new ServiceCollection();
        collection.Resolve();
        collection.AddSingleton<MainWindow>();

        ServiceProvider services = collection.BuildServiceProvider();

        IStreaming streaming = services.GetRequiredService<IStreaming>();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow(streaming);
        }

        base.OnFrameworkInitializationCompleted();
    }
}