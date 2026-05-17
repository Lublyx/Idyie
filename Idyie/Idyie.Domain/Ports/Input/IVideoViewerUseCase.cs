using Idyie.Domain.ValueObjects;

namespace Idyie.Domain.Ports.Input;

public interface IVideoViewerUseCase
{

    public Task StartVideoViewer(Action<VideoData> action, CancellationToken token);

    public void EndVideoViewer();
}
