using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using SkiaSharp;
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Sources;
using VisioForge.DotNet.VideoCapture;

namespace IdyieUI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Video();
    }

    private async void Video()
    {

        VideoCaptureCoreX videoCapture = new VideoCaptureCoreX(videoView);

        VisioForge.Core.Types.X.Sources.VideoCaptureDeviceInfo[] devices = await DeviceEnumerator.Shared.VideoSourcesAsync();
        videoCapture.Video_Source = new VideoCaptureDeviceSourceSettings(devices.First());

        
        await videoCapture.StartAsync();
    }

}