using Idyie.Domain.Ports.Output;
using Idyie.Domain.ValueObjects;
using OpenCvSharp;

namespace Idyie.Infrastructure.OpenCvSharp;

public class VideoRecording : IVideoRecording
{
    private VideoCapture? _videoCapture;
    private bool _isRunning = false;
    private Action<VideoData>? _callback;
    private CancellationToken? _token;

    public async Task StartRecording(Action<VideoData> callback, CancellationToken token)
    {
        _callback = callback;
        _token = token;

        
        _videoCapture = new VideoCapture(0, VideoCaptureAPIs.V4L2);

        _isRunning = true;
        Thread captureThread = new Thread(CaptureLoop) { IsBackground = true };
        captureThread.Start();
    }

    private void CaptureLoop()
    {
        using Mat frame = new Mat();
        using Mat gray = new Mat();
        using Mat bgra = new Mat();

        while (_isRunning && !(bool)_token?.IsCancellationRequested!)
        {
            try
            {
                _videoCapture!.Read(frame);
                if (frame.Empty()) continue;

                Cv2.CvtColor(frame, bgra, ColorConversionCodes.BGR2BGRA);

                VideoData videoData = BuildAvaloniaVideoData(bgra);
                
                _callback?.Invoke(videoData);

                Thread.Sleep(33);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error : {ex.Message}");
            }
        }

        _videoCapture?.Release();
    }

    private VideoData BuildAvaloniaVideoData(Mat bgra)
    {
        Mat bgr = new Mat();
        Cv2.CvtColor(bgra, bgr, ColorConversionCodes.BGRA2BGR);
        Cv2.ImEncode(".jpg", bgr, out byte[] jpg);
        VideoData videoData = new VideoData()
        {
            W = bgra.Width,
            H = bgra.Height,
            Size = jpg.Length,
            Pixels = jpg 
        };

        return videoData;
    }
}
