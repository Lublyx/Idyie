using System.Runtime.InteropServices;
using Idyie.Domain.Ports.Output;
using Idyie.Domain.ValueObjects;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace Idyie.Infrastructure.Onnx;

public class ObjectDetection : IObjectDetection
{

    private const string MODEL_PATH = "yolov8s.onnx";
    private readonly InferenceSession _inferenceSession;
    private readonly float[] _data = new float[1 * 3 * 640 * 640];
    private readonly DenseTensor<float> _tensor;
    private List<ObjectDetected> _objectDetecteds = new();
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

    public ObjectDetection()
    {
        _tensor = new DenseTensor<float>(_data, new[] { 1, 3, 640, 640 });
        var opts = new SessionOptions();
        opts.AppendExecutionProvider_CPU();

        _inferenceSession = new InferenceSession(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MODEL_PATH), opts);
    }

    public List<ObjectDetected> DetectObjects(byte[] frameData, float threshold = 0.75f)
    {
        Mat frame = Cv2.ImDecode(frameData, ImreadModes.Color);

        DenseTensor<float> tensor = MatToTensor(frame);

        List<NamedOnnxValue> inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("images", tensor)
        };

        using var results = _inferenceSession.Run(inputs);
        var data = results.FirstOrDefault();
        if (data == null) return [];
        float[] output = data.AsEnumerable<float>().ToArray();

        return ParseYoloOutput(output, frame.Width, frame.Height, threshold);
    }

    private DenseTensor<float> MatToTensor(Mat mat)
    {
        using Mat blob = CvDnn.BlobFromImage(
            mat,
            1.0 / 255.0,
            new Size(640, 640),
            new Scalar(),
            swapRB: true,
            crop: false
        );

        Marshal.Copy(blob.Data, _data, 0, _data.Length);

        return _tensor;
    }

    private List<ObjectDetected> ParseYoloOutput(float[] output, int w, int h, float threshold)
    {
        _objectDetecteds.Clear();
        int strid = 8400;
        int numClasses = _cocoLabels.Length;
        float scaleX = (float)w / 640;
        float scaleY = (float)h / 640;

        for (int i = 0; i < strid; i++)
        {
            int baseCx = i;
            int baseCy = i + strid;
            int baseW = i + 2 * strid;
            int baseH = i + 3 * strid;

            float cx = output[baseCx];
            float cy = output[baseCy];
            float newW = output[baseW];
            float newH = output[baseH];

            float maxScore = 0f;
            int classId = -1;
            int classOffset = 4 * strid;
            for (int c = 0; c < numClasses; c++)
            {
                float score = output[classOffset + c * strid + i];
                if (score > maxScore)
                {
                    maxScore = score;
                    classId = c;
                }
            }

            if (maxScore < threshold) continue;

            ObjectDetected objectDetected = new ObjectDetected
            {
                Label = _cocoLabels[classId],
                Score = maxScore,
                X = (int)((cx - newW / 2) * scaleX),
                Y = (int)((cy - newH / 2) * scaleY),
                W = (int)(newW * scaleX),
                H = (int)(newH * scaleY),
                Emotion = Status.Emotions.Normal,
                EmotionTimeOut = DateTime.Now
            };

            _objectDetecteds.Add(objectDetected);
        }
        return ApplyNMS(_objectDetecteds, 0.45f);
    }

    private List<ObjectDetected> ApplyNMS(List<ObjectDetected> detections, float iouThreshold)
    {
        // ─── Trie par score décroissant ───────────────────────────
        detections.Sort((a, b) => b.Score.CompareTo(a.Score));
        var kept = new List<ObjectDetected>();

        while (detections.Count > 0)
        {
            // Garde la meilleure détection
            var best = detections[0];
            kept.Add(best);
            detections.RemoveAt(0);

            // Supprime toutes les boxes qui chevauchent trop
            detections.RemoveAll(d =>
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
