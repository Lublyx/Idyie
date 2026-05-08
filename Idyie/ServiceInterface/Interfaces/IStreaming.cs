using System;
using Idyie.Dto;
using ServiceInterface.Services;

namespace ServiceInterface.Interfaces;

public interface IStreaming
{
    public Task StartStreaming(Action<AvaloniaVideoData> callback, CancellationToken token);
}
