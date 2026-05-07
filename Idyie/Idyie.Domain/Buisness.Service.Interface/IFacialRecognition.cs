using System;
using OpenCvSharp;

namespace Idyie.Domain.Buisness.Service.Interface;

public interface IFacialRecognition
{
    public void Analyse(Mat frame, Mat gray);
}
