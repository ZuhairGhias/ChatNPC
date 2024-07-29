using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using UnityEngine;

public class MemoryLog
{
    /// <summary>
    /// Should probably be using a priority queue instead
    /// </summary>
    public Queue<Memory> Memories { get;  }

    public int Capacity { get; }

    public MemoryLog(int capacity)
    {
        Memories = new Queue<Memory>();
        Capacity = capacity;
    }

    public void LogMemory(string content, Vector<double> embeddings)
    {
        LogMemory(new Memory(content, embeddings));
    }

    public void LogMemory(Memory memory) 
    {
        Memories.Enqueue(memory);
        while (Memories.Count > Capacity)
        {
            // Discard oldest memory
            Memories.Dequeue();
        }
    }

    public List<Memory> FetchMostRelevantMemories(int number, Vector<double> embeddings)
    {
        SortedList<Tuple<double,long>, Memory> sortedMemories = new();
        foreach (var mem in Memories)
        {
            sortedMemories.Add(new (mem.CosineSimilarity(embeddings), mem.TimeStamp.Ticks), mem);
        }
        return sortedMemories.Take(number).Select(x => x.Value).ToList();
    }

    public string FetchMemoriesToString(int number, Vector<double> embeddings)
    {
        List<Memory> relevantMemories = FetchMostRelevantMemories(number, embeddings);
        StringBuilder sb = new StringBuilder();
        foreach (var mem in relevantMemories)
        {
            sb.AppendLine(mem.ToString());
        }
        return sb.ToString();
        
    }
}
