using System;
using Idyie.Dto;

namespace Idyie.Domain.Buisness.Service.Interface;

public interface IBSVideoViewer
{

    public Task StartVideoViewer(Action<AvaloniaVideoData> action, CancellationToken token);

    public void EndVideoViewer();
}
