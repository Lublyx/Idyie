using Idyie.Domain.Buisness.Service;
using Idyie.Domain.Buisness.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using ServiceInterface.Interfaces;
using ServiceInterface.Services;

namespace ServiceInterface.Dependency.Injection;

public static class DI
{

    public static void Resolve(this IServiceCollection services)
    {
        services.AddSingleton<IVideoRecording, VideoRecording>();
        services.AddSingleton<IStreaming, Streaming>();

    }
}
