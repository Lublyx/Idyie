using System;
using System.Runtime.InteropServices;
using Idyie.Domain.Buisness.Service.Interface;
using Idyie.Dto;
using OpenCvSharp;

namespace Idyie.Domain.Buisness.Service;

public class BSFacialRecognition : IBSFacialRecognition
{
    private readonly IBSObjectDetection _bsObjectDetection;
    private CascadeClassifier? _cascadeClassifier;
    private CascadeClassifier? _cascadeClassifierProfile;

    public BSFacialRecognition(IBSObjectDetection bsObjectDetection)
    {
        _bsObjectDetection = bsObjectDetection;
        Load();
    }

    public byte[] Analyse(byte[] frameBuffer, int frameSize, int w, int h)
    {
        using Mat brg = new Mat();
        using Mat gray = new Mat();
        using Mat frame = new Mat(h, w, MatType.CV_8UC4);
        try
        {
            Marshal.Copy(frameBuffer, 0, frame.Data, frameSize);

            Cv2.CvtColor(frame, brg, ColorConversionCodes.BGRA2BGR);
            Cv2.CvtColor(brg, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.EqualizeHist(gray, gray);

            DetectFace(brg, gray);
            DetectObjects(brg, brg);

            using Mat bgra = new Mat();
            Cv2.CvtColor(brg, bgra, ColorConversionCodes.BGR2BGRA);

            byte[] result = new byte[w * h * 4];
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

    private void DetectObjects(Mat frame, Mat brg)
    {
        List<ObjectDetected> objectDetecteds = _bsObjectDetection.DetectObjects(brg);

        foreach (ObjectDetected obj in objectDetecteds)
        {
            if (!obj.ToDisplay) continue;
            Rect rect = new Rect(obj.X, obj.Y, obj.W, obj.H);
            Cv2.Rectangle(frame, rect, Scalar.White, 2);
            Cv2.PutText(frame, $"{obj.Label} : {obj.Score:P0}", new OpenCvSharp.Point(obj.X, obj.Y - 10), HersheyFonts.HersheySimplex, 0.6, Scalar.White, 2);
        }
    }

    private void Load()
    {
        _cascadeClassifier = new CascadeClassifier("/home/lucas/Documents/1-PROJET/Idyie/Idyie/IdyieUI/bin/Debug/net8.0/haarcascade_frontalface_default.xml");
        _cascadeClassifierProfile = new CascadeClassifier("/home/lucas/Documents/1-PROJET/Idyie/Idyie/IdyieUI/bin/Debug/net8.0/haarcascade_profileface.xml");
    }
}
