using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    public List<BinEntry> binStats = new List<BinEntry>
    {
        new BinEntry { binName = "Wood", counts = new BinCounts() },
        new BinEntry { binName = "Carbon", counts = new BinCounts() },
        new BinEntry { binName = "Metal", counts = new BinCounts() },
        new BinEntry { binName = "Heavy", counts = new BinCounts() } 
    };
    
    public BinCounts GetCounts(string binName)
    {
        return binStats.Find(b => b.binName.Equals(binName, StringComparison.OrdinalIgnoreCase))?.counts;
    }

 
    public void Increment(string binName, bool correct)
    {
        BinEntry entry = binStats.Find(b => b.binName.Equals(binName, StringComparison.OrdinalIgnoreCase));

        if (entry == null)
        {
            //Debug.LogWarning($"[GameState] Auto-creating bin entry for: {binName}");
            entry = new BinEntry { binName = binName, counts = new BinCounts() };
            binStats.Add(entry);
        }

        if (correct) entry.counts.correct++;
        else entry.counts.incorrect++;

        //Debug.Log($"[GameState] {binName.ToLower()} updated -> Correct: {entry.counts.correct}, Incorrect: {entry.counts.incorrect}");
    }
}

[System.Serializable]
public class BinEntry
{
    public string binName;
    public BinCounts counts;
}

[System.Serializable]
public class BinCounts
{
    public int correct = 0;
    public int incorrect = 0;
}