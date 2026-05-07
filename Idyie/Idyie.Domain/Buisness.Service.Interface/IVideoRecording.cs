using System;
using Idyie.Dto;

namespace Idyie.Domain.Buisness.Service.Interface;

public interface IVideoRecording
{
    public void StartRecording(Action<AvaloniaVideoData> callback, CancellationToken token);
}
