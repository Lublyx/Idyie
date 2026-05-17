using Idyie.Domain.ValueObjects;

namespace Idyie.Domain.Ports.Output;

public interface IVideoRecording
{
    public Task StartRecording(Action<VideoData> callback, CancellationToken token);
}
