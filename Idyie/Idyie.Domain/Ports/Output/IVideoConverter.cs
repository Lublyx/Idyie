using System;
using Idyie.Domain.ValueObjects;

namespace Idyie.Domain.Ports.Output;

public interface IVideoConverter
{
    public VideoData MatToVideoData(byte[] pixels, int frameSize);
}
