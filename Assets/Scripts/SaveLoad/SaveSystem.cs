using System;
using System.IO;
using UnityEngine;

namespace SaveLoad
{
    public static class SaveSystem
    {
        public static void SaveToFile(GameState state, string saveName)
        {
            string fileName = $"save_{saveName}.json";
            string fullPath = Path.Combine(Application.persistentDataPath, fileName);

            try
            {
                string json = JsonUtility.ToJson(state, true);
                File.WriteAllText(fullPath, json);
                //Debug.Log("[SaveSystem] Saved to: " + fullPath);
            }
            catch (Exception e)
            {
                Debug.LogError("[SaveSystem] Failed to save: " + e.Message);
            }
        }


        public static GameState LoadFromFile(string filename)
        {
            string fullPath = Path.Combine(Application.persistentDataPath, $"save_{filename}.json");

            if (!File.Exists(fullPath))
                return null;

            string json = File.ReadAllText(fullPath);
            return JsonUtility.FromJson<GameState>(json);
        }


        public static string[] GetAllSaveFiles()
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath, "*.json");
            return files;
        }
    }
}