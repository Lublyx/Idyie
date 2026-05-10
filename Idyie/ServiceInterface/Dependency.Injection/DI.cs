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
        // BS
        services.AddSingleton<IBSVideoRecording, BSVideoRecording>();
        services.AddSingleton<IBSFacialRecognition, BSFacialRecognition>();
        services.AddSingleton<IBSStreamVideo, BSStreamVideo>();
        services.AddSingleton<IBSServerRedirecting, BSServerRedirecting>();
        services.AddSingleton<IBSObjectDetection, BSObjectDetection>();

        // SI
        services.AddSingleton<ISIStreaming, SIStreaming>();
        services.AddSingleton<ISIServerUtils, SIServerUtils>();

    }
}
