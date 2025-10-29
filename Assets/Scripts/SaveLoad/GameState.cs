using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

[System.Serializable]
public class GameState
{
    [Header("Bin Sorting Stats")]
    public List<BinEntry> binStats = new List<BinEntry>
    {
        new BinEntry { binName = "Wood", counts = new BinCounts() },
        new BinEntry { binName = "Carbon", counts = new BinCounts() },
        new BinEntry { binName = "Metal", counts = new BinCounts() },
        new BinEntry { binName = "Heavy", counts = new BinCounts() }
    };

    [Header("Active Crates Runtime Data")]
    public List<CrateSnapshot> activeCrates = new List<CrateSnapshot>();

    [Header("Spawner Timer Sync")]
    public float spawnTimer = 0f;

    
    public BinCounts GetCounts(string binName)
    {
        return binStats.Find(b => b.binName.Equals(binName, StringComparison.OrdinalIgnoreCase))?.counts;
    }
    
    public void Increment(string binName, bool correct)
    {
        BinEntry entry = binStats.Find(b => b.binName.Equals(binName, StringComparison.OrdinalIgnoreCase));

        if (entry == null)
        {
            entry = new BinEntry { binName = binName, counts = new BinCounts() };
            binStats.Add(entry);
        }

        if (correct) entry.counts.correct++;
        else entry.counts.incorrect++;
    }
    
    public void CaptureActiveCrates()
    {
        activeCrates.Clear();

        foreach (var crate in GameObject.FindGameObjectsWithTag("Crate"))
        {
            var data = crate.GetComponent<BoxData>();
            if (data == null) continue;

            activeCrates.Add(new CrateSnapshot
            {
                crateType = data.boxType,
                position = crate.transform.position,
                rotation = crate.transform.rotation
            });
        }
    }
    
    public void RestoreCratesFromState()
    {
        // Destroy existing crates
        foreach (var crate in GameObject.FindGameObjectsWithTag("Crate"))
            UnityEngine.Object.Destroy(crate);

        // Respawn saved crates
        foreach (var snapshot in activeCrates)
        {
            var crate = Spawning.CratePoolManager.Instance.GetCrate(snapshot.crateType, snapshot.position);
            if (crate == null) continue;

            crate.transform.rotation = snapshot.rotation;

            var data = crate.GetComponent<BoxData>();
            data?.SetType(snapshot.crateType);

            var loader = crate.GetComponent<Streaming.BoxTextureLoader>();
            loader?.LoadTextureFromStreamingAssets();
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

[System.Serializable]
public class CrateSnapshot
{
    public string crateType;
    public Vector3 position;
    public Quaternion rotation;
}
