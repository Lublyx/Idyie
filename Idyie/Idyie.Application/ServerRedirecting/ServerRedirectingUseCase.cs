using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Channels;
using Idyie.Domain.Ports.Input;
using Idyie.Domain.Ports.Output;

namespace Idyie.Application.ServerRedirecting;

public class ServerRedirectingUseCase : IServerRedirectingUseCase
{

    private readonly IRecognition _recognition;
    private readonly Channel<(byte[] size, byte[] frame, int frameSize)> _channel = Channel.CreateBounded<(byte[], byte[], int)>(new BoundedChannelOptions(1)
    {
        FullMode = BoundedChannelFullMode.DropOldest,
        SingleReader = true,
        SingleWriter = true
    });
    private readonly Channel<(byte[] frameBuffer, int frameSize)> _rowChannel = Channel.CreateBounded<(byte[], int)>(new BoundedChannelOptions(1)
    {
        FullMode = BoundedChannelFullMode.DropOldest,
        SingleReader = true,
        SingleWriter = true
    });

    public ServerRedirectingUseCase(IRecognition recognition)
    {
        _recognition = recognition;
    }

    public async Task InputStreaming(TcpListener serverInput)
    {
        while (true)
        {
            TcpClient clientInput = await serverInput.AcceptTcpClientAsync();
            NetworkStream streamInput = clientInput.GetStream();

            byte[] bufferSize = new byte[4];
            _ = Task.Run(AnalyseAsync);

            try
            {
                while (true)
                {
                    await ReadExectAsync(streamInput, bufferSize, 0, 4);
                    int frameSize = BitConverter.ToInt32(bufferSize, 0);
                    byte[] frameBuffer = new byte[frameSize];

                    if (frameSize <= 0 || frameSize > 10_000_000)
                    {
                        Console.WriteLine($"Erreur : {frameSize}");
                        break;
                    }

                    if (frameBuffer.Length < frameSize)
                        frameBuffer = new byte[frameSize];

                    await ReadExectAsync(streamInput, frameBuffer, 0, frameSize);

                    await _rowChannel.Writer.WriteAsync((frameBuffer[..frameSize], frameSize));

                }
            }
            catch
            {
                Console.WriteLine("Sender disconected");
            }
            finally
            {
                clientInput.Dispose();
            }
        }
    }

    private async Task AnalyseAsync()
    {
        await foreach (var (buffer, size) in _rowChannel.Reader.ReadAllAsync())
        {
            byte[] analysedFrame = _recognition.Analyse(buffer, size);
            int analysedFrameSize = analysedFrame.Length;
            byte[] newByteSize = BitConverter.GetBytes(analysedFrameSize);

            await _channel.Writer.WriteAsync((newByteSize, analysedFrame, analysedFrameSize));
        }
    }

    public async Task OutputStreaming(TcpListener serverOutput)
    {
        while (true)
        {
            TcpClient clientOutput = await serverOutput.AcceptTcpClientAsync();
            NetworkStream streamOutput = clientOutput.GetStream();

            try
            {
                await foreach (var (size, frame, frameSize) in _channel.Reader.ReadAllAsync())
                {
                    long writeStart = Stopwatch.GetTimestamp();
                    await streamOutput.WriteAsync(size);
                    await streamOutput.WriteAsync(frame.AsMemory(0, frameSize));
                    long writeEnd = Stopwatch.GetTimestamp();

                    Console.WriteLine($"[SERVER] Write: {(writeEnd - writeStart) / 10000}ms | FrameSize: {frameSize}b");
                }
            }
            catch
            {
                Console.WriteLine("Viewer disconected");
            }
            finally
            {
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
                throw new EndOfStreamException("Disconected");

            totalRead += read;
        }
    }
}
