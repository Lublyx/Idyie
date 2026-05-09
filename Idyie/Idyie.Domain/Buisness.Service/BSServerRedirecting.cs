using System;
using System.Net.Sockets;
using System.Threading.Channels;
using Idyie.Domain.Buisness.Service.Interface;

namespace Idyie.Domain.Buisness.Service;

public class BSServerRedirecting : IBSServerRedirecting
{

    private readonly Channel<(byte[] size, byte[] frame, int frameSize)> _channel = Channel.CreateBounded<(byte[], byte[], int)>(new BoundedChannelOptions(1)
    {
        FullMode = BoundedChannelFullMode.DropOldest,
        SingleReader = true,
        SingleWriter = true
    });
    public async Task InputStreaming(TcpListener serverInput)
    {
        while (true)
        {
            TcpClient clientInput = await serverInput.AcceptTcpClientAsync();
            NetworkStream streamInput = clientInput.GetStream();

            byte[] bufferSize = new byte[4];
            byte[] frameBuffer = new byte[680 * 480 * 4];

            try
            {
                while (true)
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
                    await streamOutput.WriteAsync(size);
                    await streamOutput.WriteAsync(frame.AsMemory(0, frameSize));
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
