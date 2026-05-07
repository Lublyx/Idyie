using System;
using Idyie.Domain.Buisness.Service;
using Idyie.Domain.Buisness.Service.Interface;
using Idyie.Dto;
using ServiceInterface.Interfaces;

namespace ServiceInterface.Services;

public class Streaming : IStreaming
{
    private IVideoRecording _videoRecording;

    public Streaming(IVideoRecording videoRecording)
    {
        _videoRecording = videoRecording;
    }

    public void StartStreaming(Action<AvaloniaVideoData> callback, CancellationToken token)
    {
        try
        {
            _videoRecording.StartRecording(callback, token);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine("Error : " + ex);
        }
    }
}
