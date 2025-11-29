using System;
using UnityEngine;

namespace SaveLoad
{
    [Serializable]
    public class SaveData
    {
        public int saveVersion = 1;

        public string profileName = "Player";

        public int totalSortedBoxes;
        public int totalIncorrectBoxes;
        public int hazardsAverted; 
        
        public long totalPlaySeconds;

    }
}