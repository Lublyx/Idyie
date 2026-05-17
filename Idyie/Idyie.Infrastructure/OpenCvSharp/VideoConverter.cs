using System.Runtime.InteropServices;
using Idyie.Domain.Ports.Output;
using Idyie.Domain.ValueObjects;
using OpenCvSharp;

namespace Idyie.Infrastructure.OpenCvSharp;

public class VideoConverter : IVideoConverter
{

    public VideoData MatToVideoData(byte[] pixels, int frameSize)
    {
        Mat decoded = Cv2.ImDecode(pixels[..frameSize], ImreadModes.Color);

        Mat bgra = new Mat();
        Cv2.CvtColor(decoded, bgra, ColorConversionCodes.BGR2BGRA);

        VideoData videoData = new VideoData()
        {
            W = bgra.Width,
            H = bgra.Height,
            Size = bgra.Width * bgra.Height * 4,
            Pixels = new byte[bgra.Width * bgra.Height * 4]
        };
        Marshal.Copy(bgra.Data, videoData.Pixels, 0, videoData.Size);

        return videoData;
    }
}
