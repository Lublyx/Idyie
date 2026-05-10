using System;
using OpenCvSharp;

namespace Idyie.Domain.Buisness.Service.Interface;

public interface IBSFacialRecognition
{
    public byte[] Analyse(byte[] frameBuffer, int frameSize, int w, int h);
}
