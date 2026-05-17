using System;
using System.Runtime.InteropServices;
using Idyie.Domain.Buisness.Service.Interface;
using Idyie.Dto;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.Trainers;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace Idyie.Domain.Buisness.Service;

public class BSObjectDetection : IBSObjectDetection
{

    private const string MODEL_PATH = "yolov8s.onnx";
    private readonly InferenceSession _inferenceSession;
    private readonly string[] _cocoLabels = {
    "person",
    "bicycle",
    "car",
    "motorcycle",
    "airplane",
    "bus",
    "train",
    "truck",
    "boat",
    "traffic light",
    "fire hydrant",
    "stop sign", "parking meter",
    "bench",
    "bird",
    "cat",
    "dog",
    "horse",
    "sheep",
    "cow",
    "elephant",
    "bear",
    "zebra",
    "giraffe",
    "backpack",
    "umbrella",
    "handbag",
    "tie",
    "suitcase",
    "frisbee",
    "skis",
    "snowboard",
    "sports ball",
    "kite",
    "baseball bat",
    "baseball glove",
    "skateboard",
    "surfboard",
    "tennis racket",
    "bottle",
    "wine glass",
    "cup",
    "fork",
    "knife",
    "spoon",
    "bowl",
    "banana",
    "apple",
    "sandwich",
    "orange",
    "broccoli",
    "carrot",
    "hot dog",
    "pizza",
    "donut",
    "cake",
    "chair",
    "couch",
    "potted plant",
    "bed",
    "dining table",
    "toilet",
    "tv",
    "laptop",
    "mouse",
    "remote",
    "keyboard",
    "cell phone",
    "microwave",
    "oven",
    "toaster",
    "sink",
    "refrigerator",
    "book",
    "clock",
    "vase",
    "scissors",
    "teddy bear",
    "hair drier",
    "toothbrush"
    };

    public BSObjectDetection()
    {
        _inferenceSession = new InferenceSession(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MODEL_PATH));
    }

    public List<ObjectDetected> DetectObjects(Mat frame, float threshold = 0.75f)
    {
        DenseTensor<float> tensor = MatToTensor(frame);

        List<NamedOnnxValue> inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("images", tensor)
        };

        using var results = _inferenceSession.Run(inputs);
        var data = results.FirstOrDefault();
        if (data == null) return [];
        Tensor<float> output = data.AsTensor<float>();

        return ParseYoloOutput(output, frame.Width, frame.Height, threshold);
    }

    private DenseTensor<float> MatToTensor(Mat mat)
    {
        using Mat blob = CvDnn.BlobFromImage(
            mat,
            1.0 / 255.0,
            new OpenCvSharp.Size(640, 640),
            new Scalar(),
            swapRB: true,
            crop: false
        );

        int[] size = [1, 3, 640, 640];
        float[] data = new float[1 * 3 * 640 * 640];
        Marshal.Copy(blob.Data, data, 0, data.Length);

        return new DenseTensor<float>(data, size);
    }

    private List<ObjectDetected> ParseYoloOutput(Tensor<float> output, int w, int h, float threshold)
    {
        List<ObjectDetected> objectDetecteds = new List<ObjectDetected>();
        int numAnchors = output.Dimensions[2];
        int numClasses = _cocoLabels.Length;
        float scaleX = (float)w / 640;
        float scaleY = (float)h / 640;

        for (int i = 0; i < numAnchors; i++)
        {
            float cx = output[0, 0, i];
            float cy = output[0, 1, i];
            float newW = output[0, 2, i];
            float newH = output[0, 3, i];

            float maxScore = 0f;
            int classId = -1;
            for (int c = 0; c < numClasses; c++)
            {
                float score = output[0, 4 + c, i];
                if (score > maxScore)
                {
                    maxScore = score;
                    classId = c;
                }
            }

            if (maxScore < threshold) continue;

            objectDetecteds.Add(new ObjectDetected
            {
                Label = _cocoLabels[classId],
                Score = maxScore,
                X = (int)((cx - newW / 2) * scaleX),
                Y = (int)((cy - newH / 2) * scaleY),
                W = (int)(newW * scaleX),
                H = (int)(newH * scaleY),
                ToDisplay = Status.DangerObjects.Contains<string>(_cocoLabels[classId]) || _cocoLabels[classId] == "person"
            });
        }
        return ApplyNMS(objectDetecteds, 0.45f);
    }

    private List<ObjectDetected> ApplyNMS(List<ObjectDetected> detections, float iouThreshold)
    {
        // ─── Trie par score décroissant ───────────────────────────
        var sorted = detections.OrderByDescending(d => d.Score).ToList();
        var kept = new List<ObjectDetected>();

        while (sorted.Count > 0)
        {
            // Garde la meilleure détection
            var best = sorted[0];
            kept.Add(best);
            sorted.RemoveAt(0);

            // Supprime toutes les boxes qui chevauchent trop
            sorted.RemoveAll(d =>
                d.Label == best.Label &&          // même classe
                IoU(best, d) > iouThreshold);     // trop superposées
        }

        return kept;
    }

    private static float IoU(ObjectDetected a, ObjectDetected b)
    {
        // Intersection
        int x1 = Math.Max(a.X, b.X);
        int y1 = Math.Max(a.Y, b.Y);
        int x2 = Math.Min(a.X + a.W, b.X + b.W);
        int y2 = Math.Min(a.Y + a.H, b.Y + b.H);

        int intersectionArea = Math.Max(0, x2 - x1) * Math.Max(0, y2 - y1);
        if (intersectionArea == 0) return 0;

        // Union
        int aArea = a.W * a.H;
        int bArea = b.W * b.H;
        int unionArea = aArea + bArea - intersectionArea;

        return (float)intersectionArea / unionArea;
    }
}
