using System;
using OpenCvSharp;
using System.Runtime.InteropServices;
using Avalonia.Threading;
using Avalonia.Media.Imaging;
using Avalonia;
using Avalonia.Platform;
using System.Threading;
using System.Collections.Generic;

namespace IdyieUI;

public partial class MainWindow : Avalonia.Controls.Window
{

    private VideoCapture? _videoCapture;
    private CascadeClassifier? _cascadeClassifier;
    private CascadeClassifier? _cascadeClassifierProfile;
    private bool _isRunning = false;
    private Thread? _captureThread;

    public MainWindow()
    {
        InitializeComponent();

        Opened += StartVideo;

    }

    private async void StartVideo(object? sender, EventArgs e)
    {
        _cascadeClassifier = new CascadeClassifier("/home/lucas/Documents/1-PROJET/Idyie/Idyie/IdyieUI/bin/Debug/net8.0/haarcascade_frontalface_default.xml");
        _cascadeClassifierProfile = new CascadeClassifier("/home/lucas/Documents/1-PROJET/Idyie/Idyie/IdyieUI/bin/Debug/net8.0/haarcascade_profileface.xml");
        _videoCapture = new VideoCapture(0, VideoCaptureAPIs.V4L2);

        _isRunning = true;
        _captureThread = new Thread(CaptureLoop) { IsBackground = true };
        _captureThread.Start();
    }

    private void CaptureLoop()
    {
        using var frame = new Mat();
        using var gray = new Mat();
        using var bgra = new Mat();

        while (_isRunning)
        {
            try
            {
                _videoCapture!.Read(frame);
                if (frame.Empty()) continue;

                Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);

                OpenCvSharp.Rect[] facesDefault = _cascadeClassifier!.DetectMultiScale(
                    frame, scaleFactor: 1.3, minNeighbors: 5);

                OpenCvSharp.Rect[] facesProfile = _cascadeClassifierProfile!.DetectMultiScale(
                    frame, scaleFactor: 1.3, minNeighbors: 5);

                IList<OpenCvSharp.Rect> faces = [.. facesDefault, .. facesProfile];


                foreach (OpenCvSharp.Rect face in faces)
                    Cv2.Rectangle(frame, face, Scalar.Yellow, 2);

                Cv2.CvtColor(frame, bgra, ColorConversionCodes.BGR2BGRA);

                Cv2.ImShow("Test", frame);

                int w = bgra.Width;
                int h = bgra.Height;
                int size = w * h * 4;
                byte[] pixels = new byte[size];
                Marshal.Copy(bgra.Data, pixels, 0, size);

                Dispatcher.UIThread.Post(() =>
                {

                    WriteableBitmap bitmap = new WriteableBitmap(
                        new PixelSize(w, h),
                        new Vector(96, 96),
                        PixelFormat.Bgra8888,
                        AlphaFormat.Opaque);


                    using var buf = bitmap.Lock();
                    Marshal.Copy(pixels, 0, buf.Address, size);
                    imageVideo.Source = bitmap;

                }, DispatcherPriority.Render);

                Thread.Sleep(33);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        _videoCapture?.Release();
    }



}