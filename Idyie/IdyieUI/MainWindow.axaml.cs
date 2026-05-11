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
using System.Net;
using Avalonia.Media;
using System.Net.Sockets;
using System.Linq;
using System.Threading.Tasks;
using System.Buffers;
using System.IO;

namespace IdyieUI;

public partial class MainWindow : Avalonia.Controls.Window
{

    private readonly ISIStreaming _streaming;


    public MainWindow(ISIStreaming streaming)
    {
        InitializeComponent();
        _streaming = streaming;

        BtnStartRecording.Click += StartVideo;
        BtnStopRecording.Click += StopVideo;
    }

    private async void StartVideo(object? sender, EventArgs e)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        TaskCompletionSource task = new TaskCompletionSource();

        await _streaming.StartVideoViewer((data =>
        {
            DisplayAvaloniaData(data);
        }), cancellationTokenSource.Token);

        await task.Task;
        cancellationTokenSource.Dispose();
    }

    private void StopVideo(object? sender, EventArgs e)
    {
        _streaming.EndVideoViewer();
    }

    private void DisplayAvaloniaData(AvaloniaVideoData videoData)
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