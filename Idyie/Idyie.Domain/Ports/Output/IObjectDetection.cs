
using Idyie.Domain.ValueObjects;

namespace Idyie.Domain.Ports.Output;

public interface IObjectDetection
{
    public List<ObjectDetected> DetectObjects(Mat frame, float threshold = 0.75f);
}
