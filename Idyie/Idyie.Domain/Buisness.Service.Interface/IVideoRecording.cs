using System;
using Idyie.Dto;

namespace Idyie.Domain.Buisness.Service.Interface;

public interface IVideoRecording
{
    public Task StartRecording(Action<AvaloniaVideoData> callback, CancellationToken token);
}
