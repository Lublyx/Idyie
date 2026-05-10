using System;
using System.Net;
using System.Net.Sockets;
using Idyie.Domain.Buisness.Service.Interface;

namespace Idyie.Domain.Buisness.Service;

public class BSStreamVideo : IBSStreamVideo
{
    private readonly IBSVideoRecording _bsVideoRecording;
    private const string _ipAdress = "127.0.0.1";
    private const int _port = 5001;
    private readonly SemaphoreSlim _sendThrottle = new(1, 1);

    public BSStreamVideo(IBSVideoRecording bsVideoRecording)
    {
        _bsVideoRecording = bsVideoRecording;
    }

    public async Task StreamVideo()
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(_ipAdress), _port);
        TcpClient tcpClient = new()
        {
            NoDelay = true
        };

        await tcpClient.ConnectAsync(endPoint);
        NetworkStream stream = tcpClient.GetStream();

        CancellationTokenSource cts = new CancellationTokenSource();
        TaskCompletionSource awaitTask = new TaskCompletionSource();


        await _bsVideoRecording.StartRecording(async data =>
        {
            if (!await _sendThrottle.WaitAsync(0)) return;
            try
            {
                byte[] wBytes = BitConverter.GetBytes(data.W);
                byte[] hBytes = BitConverter.GetBytes(data.H);
                byte[] size = BitConverter.GetBytes(data.Pixels.Length);
                await stream.WriteAsync(wBytes);
                await stream.WriteAsync(hBytes);
                await stream.WriteAsync(size);
                await stream.WriteAsync(data.Pixels);
            }
            catch
            {
                await cts.CancelAsync();
                awaitTask.TrySetException(new Exception("Server disconected"));
            }
            finally
            {
                _sendThrottle.Release();
            }
        }, cts.Token);

        await awaitTask.Task;
        tcpClient.Dispose();
        cts.Dispose();
    }
}
