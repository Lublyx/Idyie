using System;

namespace Idyie.Domain.ValueObjects;

public class FaceEmbedding
{
    public required float[] DataFaceEmbedding { get; set; }

    public bool Compare(float[] embedding)
    {
        float dot = 0;

        for (int i = 0; i < DataFaceEmbedding.Length; i++)
        {
            dot += DataFaceEmbedding[i] * embedding[i];
        }

        Console.WriteLine(dot);
        return dot > 0.8f;
    }
}
