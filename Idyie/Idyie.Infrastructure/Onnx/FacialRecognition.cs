
using System.Runtime.InteropServices;
using Idyie.Domain.Ports.Output;
using Idyie.Domain.ValueObjects;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;

namespace Idyie.Infrastructure.Onnx;

public class FacialRecognition : IFacialRecognition
{
    private readonly InferenceSession _session = new InferenceSession("/home/lucas/Documents/1-PROJET/Idyie/Idyie/arcfaceresnet100-8.onnx");
    private readonly CascadeClassifier _cascadeClassifier = new CascadeClassifier("/home/lucas/Documents/1-PROJET/Idyie/Idyie/IdyieUI/bin/Debug/net8.0/haarcascade_frontalface_default.xml");
    private readonly CascadeClassifier _cascadeClassifierProfile = new CascadeClassifier("/home/lucas/Documents/1-PROJET/Idyie/Idyie/IdyieUI/bin/Debug/net8.0/haarcascade_profileface.xml");

    public List<FaceEmbedding> ExtractEmbedding(List<ObjectDetected> objectDetecteds, byte[] pixels)
    {
        List<FaceEmbedding> faceEmbeddings = new List<FaceEmbedding>();
        foreach (ObjectDetected obj in objectDetecteds)
        {
            if (!obj.ToDisplay()) continue;

            FaceEmbedding faceEmbedding = new FaceEmbedding()
            {
                DataFaceEmbedding = ExtractEmbedding(pixels)
            };
            faceEmbeddings.Add(faceEmbedding);
        }
        return faceEmbeddings;
    }

    public float[] ExtractEmbedding(byte[] pixels)
    {
        using Mat face = DetectFace(Cv2.ImDecode(pixels, ImreadModes.Color));
        if (face == null || face.Empty()) return [];

        DenseTensor<float> tensor = new DenseTensor<float>(new[] { 1, 3, 112, 112 });

        for (int y = 0; y < 112; y++)
        {
            for (int x = 0; x < 112; x++)
            {
                Vec3b color = face.At<Vec3b>(y, x);

                byte b = color.Item0;
                byte g = color.Item1;
                byte r = color.Item2;

                tensor[0, 0, y, x] = (r / 255f - 0.5f) / 0.5f;
                tensor[0, 1, y, x] = (g / 255f - 0.5f) / 0.5f;
                tensor[0, 2, y, x] = (b / 255f - 0.5f) / 0.5f;
            }
        }

        List<NamedOnnxValue> inputs = new List<NamedOnnxValue>{
                NamedOnnxValue.CreateFromTensor("data", tensor)
            };

        using var results = _session.Run(inputs);
        float[] embedding = results[0].AsEnumerable<float>().ToArray();
        return L2Normalize(embedding);
    }

    private float[] L2Normalize(float[] embedding)
    {
        float sum = 0;

        for (int i = 0; i < embedding.Length; i++)
            sum += embedding[i] * embedding[i];

        float norm = MathF.Sqrt(sum);

        for (int i = 0; i < embedding.Length; i++)
            embedding[i] /= norm;

        return embedding;
    }

    private Mat DetectFace(/*Mat frame,*/ Mat bgr)
    {
        using Mat gray = new Mat();
        Cv2.CvtColor(bgr, gray, ColorConversionCodes.BGR2GRAY);
        Cv2.EqualizeHist(gray, gray);

        Rect[] facesDefault = _cascadeClassifier!.DetectMultiScale(
                gray, scaleFactor: 1.1, minNeighbors: 5);

        // Rect[] facesProfile = _cascadeClassifierProfile!.DetectMultiScale(
        //     gray, scaleFactor: 1.1, minNeighbors: 5);


        // IList<Rect> faces = [.. facesDefault, .. facesProfile];

        using Mat faceResize = new Mat();
        foreach (Rect face in facesDefault)
        {
            using Mat faceRoi = new Mat(bgr, face);

            Cv2.Resize(faceRoi, faceResize, new Size(112, 112));

        }
        return faceResize.Clone(); // Penser a changer le return pour avoir toutes les faces
    }
}

//     Cv2.Rectangle(frame, face, _emotionStatus == Status.Emotions.Danger ? Scalar.Red : Scalar.Yellow, 2);