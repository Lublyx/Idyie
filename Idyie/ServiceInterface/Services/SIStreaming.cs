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

    public async Task<bool> StartStreaming()
    {
        try
        {
            await _bsStreamVideo.StreamVideo();
        }
        catch (System.Exception ex)
        {
            Console.WriteLine("Error : " + ex);
            return false;
        }
        return true;
    }
}
