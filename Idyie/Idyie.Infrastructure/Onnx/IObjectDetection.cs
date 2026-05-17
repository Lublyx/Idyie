
using Idyie.Domain.ValueObjects;
using OpenCvSharp;

namespace Idyie.Infrastructure.Onnx;

public interface IObjectDetection
{
    public List<ObjectDetected> DetectObjects(Mat frameData, float threshold = 0.75f);
}
