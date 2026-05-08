using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using ServiceInterface.Interfaces;

namespace Idyie.Tcp;

public class Server
{

    private readonly IStreaming _streaming;
    private const string _ipAdress = "127.0.0.1";
    private const int _portInput = 5001;
    private const int _portOutput = 5002;
    private readonly Channel<(byte[] size, byte[] frame, int frameSize)> _channel = Channel.CreateBounded<(byte[], byte[], int)>(new BoundedChannelOptions(1)
    {
        FullMode = BoundedChannelFullMode.DropOldest,
        SingleReader = true,
        SingleWriter = true
    });

    public Server(IStreaming streaming)
    {
        _streaming = streaming;
    }

    public async Task StartServer()
    {
        TcpListener serverInput = new TcpListener(IPAddress.Parse(_ipAdress), _portInput);
        TcpListener serverOutput = new TcpListener(IPAddress.Parse(_ipAdress), _portOutput);

        serverInput.Start();
        serverOutput.Start();

        TcpClient clientInput = await serverInput.AcceptTcpClientAsync();
        NetworkStream streamInput = clientInput.GetStream();

        await Task.WhenAll(InputStreaming(streamInput), OutputStreaming(serverOutput));
    }

    private async Task InputStreaming(NetworkStream streamInput)
    {
        byte[] bufferSize = new byte[4];
        byte[] frameBuffer = new byte[680 * 480 * 4];

        while (true)
        {
            try
            {

                await ReadExectAsync(streamInput, bufferSize, 0, 4);
                int frameSize = BitConverter.ToInt32(bufferSize, 0);

                if (frameSize <= 0 || frameSize > 10_000_000)
                {
                    Console.WriteLine($"Erreur : {frameSize}");
                    break;
                }

                if (frameBuffer.Length < frameSize)
                    frameBuffer = new byte[frameSize];

                await ReadExectAsync(streamInput, frameBuffer, 0, frameSize);

                await _channel.Writer.WriteAsync((bufferSize[..4], frameBuffer[..frameSize], frameSize));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _channel.Writer.Complete();
                break;
            }
        }
    }

    private async Task OutputStreaming(TcpListener serverOutput)
    {
        while (true)
        {
            TcpClient clientOutput = await serverOutput.AcceptTcpClientAsync();
            NetworkStream streamOutput = clientOutput.GetStream();

            try
            {
                await foreach (var (size, frame, frameSize) in _channel.Reader.ReadAllAsync())
                {
                    await streamOutput.WriteAsync(size);
                    await streamOutput.WriteAsync(frame.AsMemory(0, frameSize));
                }
            }
            catch (System.Exception)
            {
                Console.WriteLine("Reciver Disconected");
                clientOutput.Dispose();
            }
        }
    }

    private async Task ReadExectAsync(NetworkStream stream, byte[] buffer, int offset, int count)
    {
        int totalRead = 0;

        while (totalRead < count)
        {
            int read = await stream.ReadAsync(buffer, offset + totalRead, count - totalRead);

            if (read == 0)
                Console.WriteLine("Disconected");

            totalRead += read;
        }
    }
}
