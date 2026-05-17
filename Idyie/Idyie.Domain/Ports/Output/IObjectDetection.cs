
using Idyie.Domain.ValueObjects;

namespace Idyie.Domain.Ports.Output;

public interface IObjectDetection
{
    public List<ObjectDetected> DetectObjects(byte[] frameData, float threshold = 0.75f);
}
