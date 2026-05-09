using System;
using System.Runtime.InteropServices;
using Idyie.Domain.Buisness.Service.Interface;
using Idyie.Dto;
using OpenCvSharp;

namespace Idyie.Domain.Buisness.Service;

public class BSVideoRecording : IBSVideoRecording
{
    private readonly IBSFacialRecognition _facialRecognition;
    private VideoCapture? _videoCapture;
    private bool _isRunning = false;
    private Action<AvaloniaVideoData>? _callback;
    private CancellationToken? _token;

    public BSVideoRecording(IBSFacialRecognition facialRecognition)
    {
        _facialRecognition = facialRecognition;
    }

    public async Task StartRecording(Action<AvaloniaVideoData> callback, CancellationToken token)
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

                _facialRecognition.Analyse(frame, gray);

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
