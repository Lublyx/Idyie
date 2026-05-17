
namespace Idyie.Domain.ValueObjects;

public class VideoData
{
    public int W {get; set;}
    public int H {get; set;}
    public int Size {get; set;}
    public required byte[] Pixels {get; set;}
}
