
namespace Idyie.Domain.Ports.Output;

public interface IRecognition
{
    public byte[] Analyse(byte[] frameBuffer, int frameSize);
}
