using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Analytics;

namespace SaveLoad
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        [Header("Live state sources")]
        public string currentProfileName = "Player";
        public Core.GameManager gameManager;
        public DLC.EnemyEventManager enemyManager;
        public Core.PlaytimeTracker playtimeTracker;
        
        string ProfileBase => Sanitize(currentProfileName);

        string PathMain   => Path.Combine(Application.persistentDataPath, $"{ProfileBase}_savegame.json");
        string PathBackup => Path.Combine(Application.persistentDataPath, $"{ProfileBase}_savegame.backup.json");

        public event Action<SaveData> OnSaved;
        public event Action<SaveData> OnLoaded; 
        
        
        static string Sanitize(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "Player";
            var clean = Regex.Replace(s.Trim(), @"[^a-zA-Z0-9_\-]+", "_");
            return string.IsNullOrEmpty(clean) ? "Player" : clean;
        }

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SetCurrentProfile(string profileName)
        {
            currentProfileName = profileName;
            Debug.Log($"[Save] Active profile set to '{ProfileBase}'");
            AnalyticsManager.Instance?.SetProfile(profileName);
        }

        public void LoadProfile(string profileName)
        {
            SetCurrentProfile(profileName);
            AnalyticsManager.Instance?.SetProfile(profileName);
            LoadNow();
            
        }

        public List<string> ListProfiles()
        {
            var dir = Application.persistentDataPath;
            if (!Directory.Exists(dir)) return new List<string>();

            var files = Directory.GetFiles(dir, "*_savegame.json");
            var names = files
                .Select(Path.GetFileName)
                .Where(n => !string.IsNullOrEmpty(n) && n.EndsWith("_savegame.json"))
                .Select(n => n.Substring(0, n.Length - "_savegame.json".Length))
                .Distinct()
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .ToList();

            return names;
        }

        public void SaveNow()
        {
            
            var data = Capture();
            var json = JsonUtility.ToJson(data, true);

            try
            {
                File.WriteAllText(PathMain, json);
                File.WriteAllText(PathBackup, json);
                Debug.Log($"[Save] OK → {PathMain}");
                OnSaved?.Invoke(data);
            }
            catch (Exception e)
            {
                try
                {
                    File.WriteAllText(PathBackup, json);
                    Debug.LogWarning($"[Save] Main write failed, wrote backup: {e.Message}");
                    OnSaved?.Invoke(data);
                }
                catch (Exception e2)
                {
                    Debug.LogError($"[Save] Failed to write any file: {e2.Message}");
                }
            }
        }

        public void LoadNow()
        {
            var loaded = TryLoad(PathMain) ?? TryLoad(PathBackup);
            if (loaded == null)
            {
                Debug.LogWarning("[Load] Missing/corrupt save. Applying defaults.");
                ApplyDefaults();
                return;
            }
            Apply(loaded);
        }

        private SaveData Capture()
        {
            var data = new SaveData
            {
                profileName = string.IsNullOrWhiteSpace(currentProfileName) ? "Player" : currentProfileName
            };

            if (gameManager != null)
            {
                data.totalSortedBoxes    = gameManager.GetTotalCorrectlySorted();
                data.totalIncorrectBoxes = gameManager.GetTotalIncorrectlySorted();
            }

            if (enemyManager != null)
                data.hazardsAverted = enemyManager.GetHazardsAvertedLifetime();

            if (playtimeTracker != null)
                data.totalPlaySeconds = playtimeTracker.GetTotalSeconds();
            
            return data;
        }


        private void Apply(SaveData data)
        {
            currentProfileName = data.profileName;

            gameManager?.ApplyLoadedTotals(data.totalSortedBoxes, data.totalIncorrectBoxes); 
            enemyManager?.ApplyLoadedHazardsAverted(total: data.hazardsAverted);

            if (playtimeTracker == null)
                playtimeTracker = FindObjectOfType<Core.PlaytimeTracker>(true);
            playtimeTracker?.ApplyLoadedBase(data.totalPlaySeconds);
            
            OnLoaded?.Invoke(data);  
        }


        private void ApplyDefaults()
        {
            var d = new SaveData { profileName = currentProfileName, totalPlaySeconds = 0 };
            Apply(d);                 
        }

        private SaveData TryLoad(string path)
        {
            try
            {
                if (!File.Exists(path)) return null;
                var json = File.ReadAllText(path);
                return JsonUtility.FromJson<SaveData>(json);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Load] Failed reading {path}: {e.Message}");
                return null;
            }
        }
    }
}
