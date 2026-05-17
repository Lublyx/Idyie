using System;
using System.Net;
using System.Net.Sockets;
using Idyie.Domain.Ports.Input;
using Idyie.Domain.Ports.Output;

namespace Idyie.Application.StreamVideo;

public class StreamVideoUseCase : IStreamVideoUseCase
{
    private readonly IVideoRecording _videoRecording;
    private const string _ipAdress = "127.0.0.1";
    private const int _port = 5001;
    private readonly SemaphoreSlim _sendThrottle = new(1, 1);

    public StreamVideoUseCase(IVideoRecording videoRecording)
    {
        _videoRecording = videoRecording;
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


        await _videoRecording.StartRecording(async data =>
        {
            if (!await _sendThrottle.WaitAsync(0)) return;
            try
            {
                byte[] size = BitConverter.GetBytes(data.Size);
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
