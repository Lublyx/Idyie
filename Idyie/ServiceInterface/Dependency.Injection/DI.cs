using System.Net.Security;
using Idyie.Dao;
using Idyie.Dao.Interfaces;
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
        services.AddSingleton<IBSRecognition, BSRecognition>();
        services.AddSingleton<IBSStreamVideo, BSStreamVideo>();
        services.AddSingleton<IBSServerRedirecting, BSServerRedirecting>();
        services.AddSingleton<IBSObjectDetection, BSObjectDetection>();
        services.AddSingleton<IBSVideoViewer, BSVideoViewer>();
        services.AddSingleton<IBSFacialRecognition, BSFacialRecognition>();

        //DAO
        services.AddSingleton<IDaoFacialDatabase, DaoFacialDatabase>();

        // SI
        services.AddSingleton<ISIStreaming, SIStreaming>();
        services.AddSingleton<ISIServerUtils, SIServerUtils>();

    }
}
