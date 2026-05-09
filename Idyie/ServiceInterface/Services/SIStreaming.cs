using System;
using Idyie.Domain.Buisness.Service;
using Idyie.Domain.Buisness.Service.Interface;
using Idyie.Dto;
using ServiceInterface.Interfaces;

namespace ServiceInterface.Services;

public class SIStreaming : ISIStreaming
{
    private readonly IBSStreamVideo _bsStreamVideo;

    public SIStreaming(IBSStreamVideo bsStreamVideo)
    {
        _bsStreamVideo = bsStreamVideo;
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
}
