using System;

namespace Idyie.Domain.ValueObjects;

public class FaceEmbedding
{
    public required float[] DataFaceEmbedding {get; set;}

    public float Compare(float[] embedding)
    {
        float dot = 0;
        float mag1 = 0;
        float mag2 = 0;

        for (int i = 0; i < DataFaceEmbedding.Length; i++)
        {
            dot += DataFaceEmbedding[i] * embedding[i];
            mag1 += DataFaceEmbedding[i] * DataFaceEmbedding[i];
            mag2 += embedding[i] * embedding[i];
        }

        return dot / (MathF.Sqrt(mag1) * MathF.Sqrt(mag2));
    }
}
