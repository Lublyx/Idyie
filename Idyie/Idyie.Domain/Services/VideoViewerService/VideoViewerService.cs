using System;
using System.Buffers;
using System.Net.Sockets;
using Idyie.Domain.Ports.Output;
using Idyie.Domain.ValueObjects;

namespace Idyie.Domain.Services.VideoViewerService;

public class VideoViewerService : IVideoViewerService
{
    private readonly IVideoConverter _videoConverter;

    public VideoViewerService(IVideoConverter videoConverter)
    {
        _videoConverter = videoConverter;
    }

    public async Task ReadVideoData(Action<VideoData> action, NetworkStream stream)
    {
        byte[] sizeBuffer = new byte[4];
        await ReadExectAsync(stream, sizeBuffer, 4);
        int frameSize = BitConverter.ToInt32(sizeBuffer, 0);

        if (frameSize <= 0 || frameSize > 10_000_000)
        {
            Console.WriteLine($"Erreur : {frameSize}");
            return;
        }

        byte[] pixels = ArrayPool<byte>.Shared.Rent(frameSize);

        try
        {
            await ReadExectAsync(stream, pixels, frameSize);
            
            action?.Invoke(_videoConverter.MatToVideoData(pixels, frameSize));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(pixels);
        }
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
        }
    }
}
