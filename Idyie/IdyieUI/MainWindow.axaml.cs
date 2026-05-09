using System;
using System.Runtime.InteropServices;
using Avalonia.Threading;
using Avalonia.Media.Imaging;
using Avalonia;
using Avalonia.Platform;
using Idyie.Dto;
using System.Threading;
using ServiceInterface.Interfaces;
using ServiceInterface.Services;
using System.Net;
using Avalonia.Media;
using System.Net.Sockets;
using System.Linq;
using System.Threading.Tasks;
using System.Buffers;
using System.IO;

namespace IdyieUI;

public partial class MainWindow : Avalonia.Controls.Window
{

    private readonly ISIStreaming _streaming;
    private const string _ipAdress = "127.0.0.1";
    private const int _port = 5002;
    private TcpClient? _tcpClient;
    private bool _isRunning = false;

    public MainWindow(ISIStreaming streaming)
    {
        InitializeComponent();
        _streaming = streaming;

        BtnStartRecording.Click += StartVideo;
        BtnStopRecording.Click += StopVideo;
    }

    private async void StartVideo(object? sender, EventArgs e)
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

                ParsFormAvalonia(avaloniaVideoData);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(pixels);
            }
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
            _isRunning = false;
        }
    }

    private void StopVideo(object? sender, EventArgs e)
    {
        _tcpClient?.Close();
        _tcpClient?.Dispose();
        _isRunning = false;
    }

    private void ParsFormAvalonia(AvaloniaVideoData videoData)
    {
        Dispatcher.UIThread.Post(() =>
                {
                    WriteableBitmap bitmap = new WriteableBitmap(
                        new PixelSize(videoData.W, videoData.H),
                        new Vector(96, 96),
                        PixelFormat.Bgra8888,
                        AlphaFormat.Opaque);


                    using var buf = bitmap.Lock();
                    Marshal.Copy(videoData.Pixels, 0, buf.Address, videoData.Size);
                    imageVideo.Source = bitmap;

                }, DispatcherPriority.Render);
    }
}