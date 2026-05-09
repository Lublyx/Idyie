using System;
using System.Net.Sockets;
using Idyie.Domain.Buisness.Service.Interface;
using OpenCvSharp;
using ServiceInterface.Interfaces;

namespace ServiceInterface.Services;

public class SIServerUtils : ISIServerUtils
{
    private readonly IBSServerRedirecting _bsServerRedirecting;

    public SIServerUtils (IBSServerRedirecting bsServerRedirecting)
    {
        _bsServerRedirecting = bsServerRedirecting;
    }

    public async Task StartInputStreaming(TcpListener serverInput)
    {
        try
        {
            await _bsServerRedirecting.InputStreaming(serverInput);
        }
        catch (System.Exception e)
        {
            throw new EndOfStreamException(e.Message);
        }
    }

    public async Task StartOutputStreaming(TcpListener serverOutput)
    {
        try
        {
            await _bsServerRedirecting.OutputStreaming(serverOutput);
        }
        catch (System.Exception e)
        {
            throw new EndOfStreamException(e.Message);
        }
    }
}
