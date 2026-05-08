using System;
using System.Net;
using System.Net.Sockets;
using ServiceInterface.Interfaces;

namespace IdyieCLI;

public class App
{
    private readonly IStreaming _streaming;
    private const string _ipAdress = "127.0.0.1";
    private const int _port = 5001;
    private readonly SemaphoreSlim _sendThrottle = new(1, 1);

    public App(IStreaming streaming)
    {
        _streaming = streaming;
    }

    public async Task Run()
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(_ipAdress), _port);
        TcpClient tcpClient = new()
        {
            NoDelay = true
        };

        await tcpClient.ConnectAsync(endPoint);
        await using NetworkStream stream = tcpClient.GetStream();

        CancellationTokenSource cts = new CancellationTokenSource();

        await _streaming.StartStreaming(async data =>
        {
            if (!await _sendThrottle.WaitAsync(0)) return;
            try
            {

                byte[] size = BitConverter.GetBytes(data.Pixels.Length);
                await stream.WriteAsync(size);
                await stream.WriteAsync(data.Pixels);
            }
            finally
            {
                _sendThrottle.Release();
            }
        }, cts.Token);

        Console.ReadLine();
        cts.Dispose();
        tcpClient.Dispose();
    }
}
