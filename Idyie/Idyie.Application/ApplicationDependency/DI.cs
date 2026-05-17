using Idyie.Application.ServerRedirecting;
using Idyie.Application.StreamVideo;
using Idyie.Application.VideoViewer;
using Idyie.Domain.Ports.Input;
using Idyie.Domain.Services.VideoViewerService;
using Microsoft.Extensions.DependencyInjection;

namespace Idyie.Application.ApplicationDependency;

public static class DI
{

    public static void Resolve(this IServiceCollection services)
    {
        services.AddSingleton<IServerRedirectingUseCase, ServerRedirectingUseCase>();
        services.AddSingleton<IStreamVideoUseCase, StreamVideoUseCase>();
        services.AddSingleton<IVideoViewerUseCase, VideoViewerUseCase>();

        //Domain Services
        services.AddSingleton<IVideoViewerService, VideoViewerService>();
    }
}
