using System;
using Idyie.Domain.Buisness.Service;
using Idyie.Domain.Buisness.Service.Interface;
using Idyie.Dto;
using ServiceInterface.Interfaces;

namespace ServiceInterface.Services;

public class Streaming : IStreaming
{
    private readonly IVideoRecording _videoRecording;

    public Streaming(IVideoRecording videoRecording)
    {
        _videoRecording = videoRecording;
    }

    public async Task StartStreaming(Action<AvaloniaVideoData> callback, CancellationToken token)
    {
        try
        {
            await _videoRecording.StartRecording(callback, token);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine("Error : " + ex);
        }
    }
}
