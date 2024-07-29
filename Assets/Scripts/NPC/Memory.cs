using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Memory
{
    public readonly string Content;
    public Vector<double> Embeddings { get;}

    public readonly DateTime TimeStamp;

    public Memory(string content, Vector<double> embeddings)
    {
        Content = content;
        Embeddings = embeddings;
        TimeStamp = DateTime.Now;
    }

    public override string ToString() { return "[" + TimeStamp.ToShortTimeString() + "] " + Content; }

    public double CosineSimilarity(Memory other)
    {
        return CosineSimilarity(other.Embeddings);
    }

    public double CosineSimilarity(Vector<double> other)
    {
        // Assuming normalized embeddings
        return Vector.Dot(Embeddings, other);
    }
}
