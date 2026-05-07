using System;
using System.Runtime.InteropServices;
using Avalonia.Threading;
using Avalonia.Media.Imaging;
using Avalonia;
using Avalonia.Platform;
using Idyie.Dto;
using System.Threading;
using ServiceInterface.Interfaces;
using ServiceInterface.Services;

namespace IdyieUI;

public partial class MainWindow : Avalonia.Controls.Window
{

    private readonly IStreaming _streaming;
    private CancellationTokenSource? _cts;

    public MainWindow(IStreaming streaming)
    {
        InitializeComponent();
        _streaming = streaming;

        BtnStartRecording.Click += StartVideo;
        BtnStopRecording.Click += StopVideo;
    }

    private async void StartVideo(object? sender, EventArgs e)
    {
        _cts = new CancellationTokenSource();

        _streaming.StartStreaming(data =>
        {
            ParsFormAvalonia(data);
        }, _cts.Token);
    }

    private void StopVideo(object? sender, EventArgs e)
    {
        _cts?.Cancel();
    }

    private void ParsFormAvalonia(AvaloniaVideoData videoData)
    {
        Dispatcher.UIThread.Post(() =>
                {
                    WriteableBitmap bitmap = new WriteableBitmap(
                        new PixelSize(videoData.W, videoData.H),
                        new Vector(96, 96),
                        PixelFormat.Bgra8888,
                        AlphaFormat.Opaque);


                    using var buf = bitmap.Lock();
                    Marshal.Copy(videoData.Pixels, 0, buf.Address, videoData.Size);
                    imageVideo.Source = bitmap;

                }, DispatcherPriority.Render);
    }
}