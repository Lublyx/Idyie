
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
        byte[] face = DetectFace(Cv2.ImDecode(pixels, ImreadModes.Color));

        DenseTensor<float> tensor = new DenseTensor<float>(new[] { 1, 3, 112, 112 });

        for (int y = 0; y < 112; y++)
        {
            for (int x = 0; x < 112; x++)
            {
                int idx = (y * 112 + x) * 3;

                tensor[0, 0, y, x] = (face[idx + 2] / 255f - 0.5f) / 0.5f;
                tensor[0, 1, y, x] = (face[idx + 1] / 255f - 0.5f) / 0.5f;
                tensor[0, 2, y, x] = (face[idx + 0] / 255f - 0.5f) / 0.5f;
            }
        }

        List<NamedOnnxValue> inputs = new List<NamedOnnxValue>{
                NamedOnnxValue.CreateFromTensor("data", tensor)
            };

        using var results = _session.Run(inputs);
        return results[0].AsEnumerable<float>().ToArray();
    }

    private byte[] DetectFace(/*Mat frame,*/ Mat brg)
    {
        Rect[] facesDefault = _cascadeClassifier!.DetectMultiScale(
                brg, scaleFactor: 1.1, minNeighbors: 5);

        // Rect[] facesProfile = _cascadeClassifierProfile!.DetectMultiScale(
        //     gray, scaleFactor: 1.1, minNeighbors: 5);


        // IList<Rect> faces = [.. facesDefault, .. facesProfile];
        byte[] bytes = new byte[112 * 112 * 3];


        foreach (Rect face in facesDefault)
        {
            using Mat faceRoi = new Mat(brg, face);
            using Mat faceResize = new Mat();

            Cv2.Resize(faceRoi, faceResize, new Size(112, 112));

            Marshal.Copy(faceResize.Data, bytes, 0, bytes.Length);
        }
        return bytes; // Penser a changer le return pour avoir toutes les faces
    }
}

//     Cv2.Rectangle(frame, face, _emotionStatus == Status.Emotions.Danger ? Scalar.Red : Scalar.Yellow, 2);