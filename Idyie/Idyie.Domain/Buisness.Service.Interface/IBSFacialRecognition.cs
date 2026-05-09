using System;
using OpenCvSharp;

namespace Idyie.Domain.Buisness.Service.Interface;

public interface IBSFacialRecognition
{
    public void Analyse(Mat frame, Mat gray);
}
