using Idyie.Dto;
using OpenCvSharp;

namespace Idyie.Domain.Buisness.Service.Interface;

public interface IBSObjectDetection
{
    public List<ObjectDetected> DetectObjects(Mat frame, float threshold = 0.8f);
}
