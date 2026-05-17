using System.Net.Security;
using Idyie.Domain.Ports.Output;
using Idyie.Infrastructure.Onnx;
using Idyie.Infrastructure.OpenCvSharp;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceInterface.Dependency.Injection;

public static class DI
{

    public static IServiceCollection ResolveInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IFacialRecognition, FacialRecognition>();
        services.AddSingleton<IObjectDetection, ObjectDetection>();
        services.AddSingleton<IRecognition, Recognition>();
        services.AddSingleton<IVideoConverter, VideoConverter>();
        services.AddSingleton<IVideoRecording, VideoRecording>();

        return services;
    }
}
