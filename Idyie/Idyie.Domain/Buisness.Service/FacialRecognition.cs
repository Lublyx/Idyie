using System;
using Idyie.Domain.Buisness.Service.Interface;
using OpenCvSharp;

namespace Idyie.Domain.Buisness.Service;

public class FacialRecognition : IFacialRecognition
{
    private CascadeClassifier? _cascadeClassifier;
    private CascadeClassifier? _cascadeClassifierProfile;

    public void Analyse(Mat frame, Mat gray)
    {
        Load();

        Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);
        Cv2.EqualizeHist(gray, gray);

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
