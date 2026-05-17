using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Idyie.Domain.Ports.Input;
using Idyie.Domain.Services.VideoViewerService;
using Idyie.Domain.ValueObjects;

namespace Idyie.Application.VideoViewer;

public class VideoViewerUseCase : IVideoViewerUseCase
{
    private readonly IVideoViewerService _videoViewerService;
    private const string _ipAdress = "127.0.0.1";
    private const int _port = 5002;
    private TcpClient? _tcpClient;
    private bool _isRunning = false;

    public VideoViewerUseCase(IVideoViewerService videoViewerService)
    {
        _videoViewerService = videoViewerService;
    }

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


        _isRunning = true;
        while (_isRunning)
        {
            await _videoViewerService.ReadVideoData(action, stream);
        }
    }

    public void EndVideoViewer()
    {
        _tcpClient?.Close();
        _tcpClient?.Dispose();
        _isRunning = false;
    }   
}
