
using Idyie.Domain.ValueObjects;

namespace Idyie.Domain.Ports.Output;

public interface IFacialRecognition
{
    public List<FaceEmbedding> ExtractEmbedding(List<ObjectDetected> objectDetecteds, byte[] pixels);

    public float[] ExtractEmbedding(byte[] pixels);
}
