
using Idyie.Domain.ValueObjects;

namespace Idyie.Domain.Ports.Output;

public interface IFacialRecognition
{
    public void DetectFace(List<ObjectDetected> objectDetecteds);
}
