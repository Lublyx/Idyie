using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Idyie.Domain.Ports.Input;
using Idyie.Domain.ValueObjects;

namespace Idyie.Domain.Buisness.Service;

public class VideoViewerUseCase : IVideoViewerUseCase
{
    private const string _ipAdress = "127.0.0.1";
    private const int _port = 5002;
    private TcpClient? _tcpClient;
    private bool _isRunning = false;

    public async Task StartVideoViewer(Action<VideoData> action, CancellationToken token)
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
                Mat decoded = Cv2.ImDecode(pixels[..frameSize], ImreadModes.Color);
                if (decoded.Empty()) continue;
                Mat bgra = new Mat();
                Cv2.CvtColor(decoded, bgra, ColorConversionCodes.BGR2BGRA);

                AvaloniaVideoData avaloniaVideoData = new AvaloniaVideoData()
                {
                    W = bgra.Width,
                    H = bgra.Height,
                    Size = bgra.Width * bgra.Height * 4,
                    Pixels = new byte[bgra.Width * bgra.Height * 4]
                };
                Marshal.Copy(bgra.Data, avaloniaVideoData.Pixels, 0, avaloniaVideoData.Size);

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
