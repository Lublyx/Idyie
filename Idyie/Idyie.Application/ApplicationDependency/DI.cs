using Idyie.Domain.Buisness.Service;
using Idyie.Domain.Ports.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Idyie.Application.ApplicationDependency;

public static class DI
{

    public static void Resolve(this IServiceCollection services)
    {
        services.AddSingleton<IServerRedirectingHandler, ServerRedirectingHandler>();
        services.AddSingleton<IStreamVideoHandler, StreamVideoHandler>();
        services.AddSingleton<IVideoViewerHandler, VideoViewerHandler>();
    }
}
