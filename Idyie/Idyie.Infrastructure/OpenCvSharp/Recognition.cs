using System.Diagnostics;
using Idyie.Domain.Ports.Output;
using Idyie.Domain.ValueObjects;
using Idyie.Infrastructure.Onnx;
using OpenCvSharp;

namespace Idyie.Infrastructure.OpenCvSharp;

public class Recognition : IRecognition
{
    private readonly IObjectDetection _objectDetection;

    public Recognition(IObjectDetection objectDetection)
    {
        _objectDetection = objectDetection;
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

            DetectObjects(frame, frame);
            // DetectFace(brg, gray);

            Cv2.ImEncode(".jpg", frame, out byte[] jpg);
            // Console.WriteLine($"[SERVER]  Face Analyse: {(endAlani - startAnali) / 10000}ms | FrameSize: {frameSize}b");

            return jpg;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return frameBuffer;
        }
    }

    // private void DetectFace(Mat frame, Mat gray)
    // {
    //     Rect[] facesDefault = _cascadeClassifier!.DetectMultiScale(
    //             gray, scaleFactor: 1.1, minNeighbors: 5);

    //     Rect[] facesProfile = _cascadeClassifierProfile!.DetectMultiScale(
    //         gray, scaleFactor: 1.1, minNeighbors: 5);

    //     IList<Rect> faces = [.. facesDefault, .. facesProfile];


    //     foreach (Rect face in faces)
    //         Cv2.Rectangle(frame, face, _emotionStatus == Status.Emotions.Danger ? Scalar.Red : Scalar.Yellow, 2);
    // }

    private void DetectObjects(Mat frame, Mat brg)
    {
        List<ObjectDetected> objectDetecteds = _objectDetection.DetectObjects(brg.ImEncode(".jpg"));

        // if (objectDetecteds.Count == 0 && _emotionTimeOut < DateTime.Now.AddSeconds(-10)) _emotionStatus = EmotionStatus.Normal;

        foreach (ObjectDetected obj in objectDetecteds)
        {
            if (!obj.ToDisplay()) continue;

            Rect rect = new Rect(obj.X, obj.Y+100, obj.W, obj.H);
            Cv2.Rectangle(frame, rect, obj.IsDanger() ? Scalar.Red : Scalar.Yellow, 2);
            Cv2.PutText(frame, $"Type : {obj.Label}; Status : {obj.Emotion}; Prediction : {obj.Score:P0}", new Point(obj.X, obj.Y - 10), HersheyFonts.HersheySimplex, 0.6, Scalar.White, 2);
        }
    }
}
