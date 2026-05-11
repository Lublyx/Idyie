using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using Idyie.Domain.Buisness.Service.Interface;
using Idyie.Dto;

namespace Idyie.Domain.Buisness.Service;

public class BSVideoViewer : IBSVideoViewer
{
    private const string _ipAdress = "127.0.0.1";
    private const int _port = 5002;
    private TcpClient? _tcpClient;
    private bool _isRunning = false;

    public async Task StartVideoViewer(Action<AvaloniaVideoData> action, CancellationToken token)
    {

        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(_ipAdress), _port);
        _tcpClient = new()
        {
            NoDelay = true
        };
        await _tcpClient.ConnectAsync(endPoint);

        await using NetworkStream stream = _tcpClient.GetStream();

        Console.WriteLine("Connected");

        byte[] sizeBuffer = new byte[4];

        _isRunning = true;
        while (_isRunning)
        {
            await ReadExectAsync(stream, sizeBuffer, 4);
            int frameSize = BitConverter.ToInt32(sizeBuffer, 0);

            if (frameSize <= 0 || frameSize > 10_000_000)
            {
                Console.WriteLine($"Erreur : {frameSize}");
                break;
            }

            byte[] pixels = ArrayPool<byte>.Shared.Rent(frameSize);

            try
            {

                await ReadExectAsync(stream, pixels, frameSize);

                AvaloniaVideoData avaloniaVideoData = new AvaloniaVideoData()
                {
                    W = 640,
                    H = 480,
                    Size = frameSize,
                    Pixels = pixels
                };

                action?.Invoke(avaloniaVideoData);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(pixels);
            }
        }
    }

    public void EndVideoViewer()
    {
        _tcpClient?.Close();
        _tcpClient?.Dispose();
        _isRunning = false;
    }

    private async Task ReadExectAsync(NetworkStream stream, byte[] buffer, int size)
    {
        try
        {
            int offset = 0;

            while (offset < size)
            {
                int read = await stream.ReadAsync(buffer, offset, size - offset);

                if (read == 0)
                    throw new EndOfStreamException("Disconected");

                offset += read;
            }
        }
        catch
        {
            Console.WriteLine("Flux closed");
            _isRunning = false;
        }
    }
}
