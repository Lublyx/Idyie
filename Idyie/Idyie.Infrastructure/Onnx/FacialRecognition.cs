
using Idyie.Domain.Ports.Output;
using Idyie.Domain.ValueObjects;

namespace Idyie.Infrastructure.Onnx;

public class FacialRecognition : IFacialRecognition
{
    public void DetectFace(List<ObjectDetected> objectDetecteds)
    {
        foreach (ObjectDetected obj in objectDetecteds)
        {
            if (!obj.ToDisplay()) continue;

            
        }
    }
}
