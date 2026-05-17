using System;
using System.Net.Sockets;
using Idyie.Domain.ValueObjects;

namespace Idyie.Domain.Services.VideoViewerService;

public interface IVideoViewerService
{

    public Task ReadVideoData(Action<VideoData> action, NetworkStream stream, byte[] sizeBuffer, bool isRunning);

}
