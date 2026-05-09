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
        services.AddSingleton<IBSVideoRecording, BSVideoRecording>();
        services.AddSingleton<ISIStreaming, SIStreaming>();
        services.AddSingleton<IBSFacialRecognition, BSFacialRecognition>();
        services.AddSingleton<IBSStreamVideo, BSStreamVideo>();

    }
}
