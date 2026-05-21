using System.Diagnostics;
using Idyie.Domain.Ports.Output;
using Idyie.Domain.ValueObjects;
using Idyie.Infrastructure.Onnx;
using OpenCvSharp;

namespace Idyie.Infrastructure.OpenCvSharp;

public class Recognition : IRecognition
{
    private readonly IObjectDetection _objectDetection;
    private readonly IFacialRecognition _facialRecognition;

    public Recognition(IObjectDetection objectDetection, IFacialRecognition facialRecognition)
    {
        _objectDetection = objectDetection;
        _facialRecognition = facialRecognition;
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
            return jpg;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return frameBuffer;
        }
    }

    private void DetectObjects(Mat frame, Mat brg)
    {
        List<ObjectDetected> objectDetecteds = _objectDetection.DetectObjects(brg.ImEncode(".jpg"));

        List<FaceEmbedding> faceEmbeddings = _facialRecognition.ExtractEmbedding(objectDetecteds, brg.ImEncode(".jpg")); // surveiller la coruption de l'image
        FaceEmbedding faceEmbeddingOrigine = new FaceEmbedding()
        {
            DataFaceEmbedding = _facialRecognition.ExtractEmbedding(Cv2.ImRead("/home/lucas/Documents/Information/portrait.jpg", ImreadModes.Color).ImEncode(".jpg"))
        };

        foreach (FaceEmbedding faceEmbedding in faceEmbeddings)
        {
            if (faceEmbedding.Compare(faceEmbeddingOrigine.DataFaceEmbedding)) Console.WriteLine("Lucas");
        }

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
