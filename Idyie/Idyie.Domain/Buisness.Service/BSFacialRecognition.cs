using System;
using System.Runtime.InteropServices;
using Idyie.Domain.Buisness.Service.Interface;
using OpenCvSharp;

namespace Idyie.Domain.Buisness.Service;

public class BSFacialRecognition : IBSFacialRecognition
{
    private CascadeClassifier? _cascadeClassifier;
    private CascadeClassifier? _cascadeClassifierProfile;

    public BSFacialRecognition()
    {
        Load();
    }

    public byte[] Analyse(byte[] frameBuffer, int frameSize)
    {
        using Mat gray = new Mat();
        using Mat frame = new Mat(480, 640, MatType.CV_8UC4);
        try
        {
            Marshal.Copy(frameBuffer, 0, frame.Data, frameSize);

            Cv2.CvtColor(frame, gray, ColorConversionCodes.BGRA2GRAY);
            Cv2.EqualizeHist(gray, gray);

            DetectFace(frame, gray);

            using Mat bgra = new Mat();
            Cv2.CvtColor(frame, bgra, ColorConversionCodes.BGR2BGRA);

            byte[] result = new byte[640 * 480 * 4];
            Marshal.Copy(bgra.Data, result, 0, result.Length);

            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return frameBuffer;
        }
    }

    private void DetectFace(Mat frame, Mat gray)
    {
        OpenCvSharp.Rect[] facesDefault = _cascadeClassifier!.DetectMultiScale(
                gray, scaleFactor: 1.1, minNeighbors: 5);

            OpenCvSharp.Rect[] facesProfile = _cascadeClassifierProfile!.DetectMultiScale(
                gray, scaleFactor: 1.1, minNeighbors: 5);

            IList<OpenCvSharp.Rect> faces = [.. facesDefault, .. facesProfile];


            foreach (OpenCvSharp.Rect face in faces)
                Cv2.Rectangle(frame, face, Scalar.Yellow, 2);
    }

    private void Load()
    {
        _cascadeClassifier = new CascadeClassifier("/home/lucas/Documents/1-PROJET/Idyie/Idyie/IdyieUI/bin/Debug/net8.0/haarcascade_frontalface_default.xml");
        _cascadeClassifierProfile = new CascadeClassifier("/home/lucas/Documents/1-PROJET/Idyie/Idyie/IdyieUI/bin/Debug/net8.0/haarcascade_profileface.xml");
    }
}
