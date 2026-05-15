using System;
using OpenCvSharp;

namespace Idyie.Domain.Buisness.Service.Interface;

public interface IBSRecognition
{
    public byte[] Analyse(byte[] frameBuffer, int frameSize, int w, int h);
}
