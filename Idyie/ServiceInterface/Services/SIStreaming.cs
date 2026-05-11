using System;
using Idyie.Domain.Buisness.Service;
using Idyie.Domain.Buisness.Service.Interface;
using Idyie.Dto;
using ServiceInterface.Interfaces;

namespace ServiceInterface.Services;

public class SIStreaming : ISIStreaming
{
    private readonly IBSStreamVideo _bsStreamVideo;
    private readonly IBSVideoViewer _bsVideoViewer;

    public SIStreaming(IBSStreamVideo bsStreamVideo, IBSVideoViewer bsVideoViewer)
    {
        _bsStreamVideo = bsStreamVideo;
        _bsVideoViewer = bsVideoViewer;
    }

    public async Task StartStreaming()
    {
        try
        {
            await _bsStreamVideo.StreamVideo();
        }
        catch
        {
            Console.WriteLine("Error : server disconected");
        }
    }

    public async Task StartVideoViewer(Action<AvaloniaVideoData> action, CancellationToken token)
    {
        try
        {
            await _bsVideoViewer.StartVideoViewer(action, token);
        }
        catch
        {
            Console.WriteLine("Error : Deconnection...");
        }
    }

    public void EndVideoViewer()
    {
        try
        {
            _bsVideoViewer.EndVideoViewer();
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"Error : {ex}");
        }
    }
}
