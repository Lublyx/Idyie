using System;
using System.Runtime.InteropServices;
using Idyie.Domain.Buisness.Service.Interface;
using Idyie.Dto;
using OpenCvSharp;

namespace Idyie.Domain.Buisness.Service;

public class VideoRecording : IVideoRecording
{
    private VideoCapture? _videoCapture;
    private CascadeClassifier? _cascadeClassifier;
    private CascadeClassifier? _cascadeClassifierProfile;
    private bool _isRunning = false;
    private Action<AvaloniaVideoData>? _callback;
    private CancellationToken? _token;

    public async void StartRecording(Action<AvaloniaVideoData> callback, CancellationToken token)
    {
        _callback = callback;
        _token = token;

        _cascadeClassifier = new CascadeClassifier("/home/lucas/Documents/1-PROJET/Idyie/Idyie/IdyieUI/bin/Debug/net8.0/haarcascade_frontalface_default.xml");
        _cascadeClassifierProfile = new CascadeClassifier("/home/lucas/Documents/1-PROJET/Idyie/Idyie/IdyieUI/bin/Debug/net8.0/haarcascade_profileface.xml");
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

                Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);
                Cv2.EqualizeHist(gray, gray);

                OpenCvSharp.Rect[] facesDefault = _cascadeClassifier!.DetectMultiScale(
                    gray, scaleFactor: 1.1, minNeighbors: 5);

                OpenCvSharp.Rect[] facesProfile = _cascadeClassifierProfile!.DetectMultiScale(
                    gray, scaleFactor: 1.1, minNeighbors: 5);

                IList<OpenCvSharp.Rect> faces = [.. facesDefault, .. facesProfile];


                foreach (OpenCvSharp.Rect face in faces)
                    Cv2.Rectangle(frame, face, Scalar.Yellow, 2);

                Cv2.CvtColor(frame, bgra, ColorConversionCodes.BGR2BGRA);

                AvaloniaVideoData videoData = BuildAvaloniaVideoData(bgra);
                
                _callback?.Invoke(videoData);

                Thread.Sleep(33);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        _videoCapture?.Release();
    }

    private AvaloniaVideoData BuildAvaloniaVideoData(Mat bgra)
    {
        AvaloniaVideoData videoData = new AvaloniaVideoData()
        {
            W = bgra.Width,
            H = bgra.Height,
            Size = bgra.Width * bgra.Height * 4,
            Pixels = new byte[bgra.Width * bgra.Height * 4],
        };
        Marshal.Copy(bgra.Data, videoData.Pixels, 0, videoData.Size);

        return videoData;
    }
}
