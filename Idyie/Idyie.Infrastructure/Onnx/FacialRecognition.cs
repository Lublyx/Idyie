
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

    public void DetectFace(List<ObjectDetected> objectDetecteds, byte[] pixels)
    {
        foreach (ObjectDetected obj in objectDetecteds)
        {
            if (!obj.ToDisplay()) continue;


            DenseTensor<float> tensor = new DenseTensor<float>(new[] {1, 3, 122, 122});

            for (int y = 0; y < 122; y++)
            {
                for (int x = 0; x < 122; x++)
                {
                    int idx = (y * 122 * x) * 3;

                    tensor[0, 0, y, x] = (pixels[idx + 2] / 255f - 0.5f) / 0.5f;
                    tensor[0, 1, y, x] = (pixels[idx + 1] / 255f - 0.5f) / 0.5f;
                    tensor[0, 2, y, x] = (pixels[idx + 0] / 255f - 0.5f) / 0.5f;
                }
            }

            List<NamedOnnxValue> inputs = new List<NamedOnnxValue>{
                NamedOnnxValue.CreateFromTensor("data", tensor)
            };

            using var results = _session.Run(inputs);
            //return results.First().AsEnumerable<float>().ToArray;
        }
    }

    private void DetectFace(/*Mat frame,*/ Mat gray)
    {
        Rect[] facesDefault = _cascadeClassifier!.DetectMultiScale(
                gray, scaleFactor: 1.1, minNeighbors: 5);

        Rect[] facesProfile = _cascadeClassifierProfile!.DetectMultiScale(
            gray, scaleFactor: 1.1, minNeighbors: 5);

        gray.Resize(0, new Scalar(112, 112));
        facesDefault.ToTensor<float>();
        Mat faces = new Mat();

        // IList<Rect> faces = [.. facesDefault, .. facesProfile];


        // foreach (Rect face in faces)
        //     Cv2.Rectangle(frame, face, _emotionStatus == Status.Emotions.Danger ? Scalar.Red : Scalar.Yellow, 2);
    }
}
