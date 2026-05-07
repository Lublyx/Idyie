using System;
using Idyie.Dto;
using ServiceInterface.Services;

namespace ServiceInterface.Interfaces;

public interface IStreaming
{
    public void StartStreaming(Action<AvaloniaVideoData> callback, CancellationToken token);
}
