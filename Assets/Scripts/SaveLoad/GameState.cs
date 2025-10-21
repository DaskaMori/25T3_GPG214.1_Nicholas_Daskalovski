using System.Collections.Generic;

[System.Serializable]
public class GameState
{
    public List<BinEntry> binStats = new List<BinEntry>
    {
        new BinEntry { binName = "Red", counts = new BinCounts() },
        new BinEntry { binName = "Green", counts = new BinCounts() },
        new BinEntry { binName = "Blue", counts = new BinCounts() }
    };

    public BinCounts GetCounts(string binName)
    {
        return binStats.Find(b => b.binName == binName)?.counts;
    }

    public void Increment(string binName, bool correct)
    {
        BinCounts target = GetCounts(binName);
        if (target != null)
        {
            if (correct) target.correct++;
            else target.incorrect++;
        }
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