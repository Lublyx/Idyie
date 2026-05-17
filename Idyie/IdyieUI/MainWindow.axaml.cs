using System;
using System.Runtime.InteropServices;
using Avalonia.Threading;
using Avalonia.Media.Imaging;
using Avalonia;
using Avalonia.Platform;
using System.Threading;
using System.Threading.Tasks;
using Idyie.Domain.Ports.Input;
using Idyie.Domain.ValueObjects;

namespace IdyieUI;

public partial class MainWindow : Avalonia.Controls.Window
{

    private readonly IVideoViewerUseCase _videoViwer;
    private WriteableBitmap _bitmap;


    public MainWindow(IVideoViewerUseCase videoViwer)
    {
        InitializeComponent();
        _videoViwer = videoViwer;

        BtnStartRecording.Click += StartVideo;
        BtnStopRecording.Click += StopVideo;
    }

    private async void StartVideo(object? sender, EventArgs e)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        TaskCompletionSource task = new TaskCompletionSource();

        await _videoViwer.StartVideoViewer((data =>
        {
            DisplayAvaloniaData(data);
        }), cancellationTokenSource.Token);

        await task.Task;
        cancellationTokenSource.Dispose();
    }

    private void StopVideo(object? sender, EventArgs e)
    {
        _videoViwer.EndVideoViewer();
    }

    private void DisplayAvaloniaData(VideoData videoData)
    {
        Dispatcher.UIThread.Post(() =>
                {
                    _bitmap = new WriteableBitmap(
                        new PixelSize(videoData.W, videoData.H),
                        new Vector(96, 96),
                        PixelFormat.Bgra8888,
                        AlphaFormat.Opaque);

                    using var buf = _bitmap.Lock();
                    Marshal.Copy(videoData.Pixels, 0, buf.Address, videoData.Size);
                    imageVideo.Source = _bitmap;

                }, DispatcherPriority.Render);
    }
}