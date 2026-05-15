using System;
using System.Runtime.InteropServices;
using Idyie.Domain.Buisness.Service.Interface;
using Idyie.Dto;
using OpenCvSharp;

namespace Idyie.Domain.Buisness.Service;

public class BSRecognition : IBSRecognition
{
    private readonly IBSObjectDetection _bsObjectDetection;
    private CascadeClassifier? _cascadeClassifier;
    private CascadeClassifier? _cascadeClassifierProfile;
    private string _emotionStatus;
    private DateTime _emotionTimeOut = DateTime.Now;

    public BSRecognition(IBSObjectDetection bsObjectDetection)
    {
        _bsObjectDetection = bsObjectDetection;
        _emotionStatus = Status.Emotions.Normal;
        Load();
    }

    public byte[] Analyse(byte[] frameBuffer, int frameSize)
    {
        using Mat brg = new Mat();
        using Mat gray = new Mat();
        using Mat frame = Cv2.ImDecode(frameBuffer, ImreadModes.Color);
        try
        {
            Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.EqualizeHist(gray, gray);

            byte[] jpgImage = frame.ImEncode(".jpg");
            DetectObjects(frame, frame, jpgImage);
            // DetectFace(brg, gray);

            Cv2.ImEncode(".jpg", frame, out byte[] jpg);
            return jpg;
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
            Cv2.Rectangle(frame, face, _emotionStatus == Status.Emotions.Danger ? Scalar.Red : Scalar.Yellow, 2);
    }

    private void DetectObjects(Mat frame, Mat brg, byte[] jpgImage)
    {

        List<ObjectDetected> objectDetecteds = _bsObjectDetection.DetectObjects(brg);

        // if (objectDetecteds.Count == 0 && _emotionTimeOut < DateTime.Now.AddSeconds(-10)) _emotionStatus = EmotionStatus.Normal;

        foreach (ObjectDetected obj in objectDetecteds)
        {
            if (Status.DangerObjects.Contains<string>(obj.Label))
            {
                _emotionStatus = Status.Emotions.Danger;
                _emotionTimeOut = DateTime.Now;
            }
            else if (_emotionTimeOut < DateTime.Now.AddSeconds(-10)) _emotionStatus = Status.Emotions.Normal;
            if (!obj.ToDisplay) continue;

            Rect rect = new Rect(obj.X, obj.Y, obj.W, obj.H);
            Cv2.Rectangle(frame, rect, _emotionStatus == Status.Emotions.Danger ? Scalar.Red : Scalar.Yellow, 2);
            Cv2.PutText(frame, $"Type : {obj.Label}; Status : {_emotionStatus}; Prediction : {obj.Score:P0}", new OpenCvSharp.Point(obj.X, obj.Y - 10), HersheyFonts.HersheySimplex, 0.6, Scalar.White, 2);
        }
    }

    private void Load()
    {
        _cascadeClassifier = new CascadeClassifier("/home/lucas/Documents/1-PROJET/Idyie/Idyie/IdyieUI/bin/Debug/net8.0/haarcascade_frontalface_default.xml");
        _cascadeClassifierProfile = new CascadeClassifier("/home/lucas/Documents/1-PROJET/Idyie/Idyie/IdyieUI/bin/Debug/net8.0/haarcascade_profileface.xml");
    }
}
