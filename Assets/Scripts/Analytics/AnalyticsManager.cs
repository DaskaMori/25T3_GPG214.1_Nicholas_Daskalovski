using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Analytics
{
    public class AnalyticsManager : MonoBehaviour
    {
        public static AnalyticsManager Instance { get; private set; }

        [Header("Batching")]
        [Tooltip("Write to disk when this many events are queued.")]
        public int flushThreshold = 20;

        [Tooltip("Always flush at this interval even if threshold not reached.")]
        public float flushIntervalSeconds = 10f;

        [Tooltip("Cap in-memory queue to avoid runaway allocations.")]
        public int maxQueue = 2000;

        [Header("Paths")]
        public string folderName = "Analytics";

        string currentProfile = "Player";
        string filePath;                  

        readonly List<AnalyticsRecord> queue = new();
        float timer;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetProfile(currentProfile);
        }

        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= flushIntervalSeconds)
            {
                timer = 0f;
                TryFlush();
            }
        }
        
        public void SetProfile(string profile)
        {
            currentProfile = string.IsNullOrWhiteSpace(profile) ? "Player" : profile.Trim();
            var dir = Path.Combine(Application.persistentDataPath, folderName);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            filePath = Path.Combine(dir, $"{currentProfile}_analytics.jsonl");
        }

        public void Enqueue(AnalyticsRecord rec)
        {
            if (rec == null) return;
            if (queue.Count >= maxQueue) queue.RemoveRange(0, Math.Max(1, queue.Count - maxQueue + 1));
            queue.Add(rec);
            if (queue.Count >= flushThreshold) TryFlush();
        }

         public void LogSort(string boxType, bool correct)
            => Enqueue(new AnalyticsRecord(currentProfile, "sort", boxType, 1, correct));

        public void LogMistake(string boxType, string targetBin)
            => Enqueue(new AnalyticsRecord(currentProfile, "mistake", $"{boxType}->{targetBin}", 1, false));

        public void LogDlc(string enemyId, string action)
            => Enqueue(new AnalyticsRecord(currentProfile, "dlc", $"{enemyId}:{action}", 1, false));

        public void LogRoundEnd(int correct, int incorrect, int hazardsAverted)
            => Enqueue(new AnalyticsRecord(currentProfile, "round_end",
                   detail: $"c={correct},i={incorrect},h={hazardsAverted}", amount: correct + incorrect, flag:false));

        public void ForceFlushNow() => TryFlush(true);

        void TryFlush(bool force = false)
        {
            if (queue.Count == 0) return;

            var batch = new List<AnalyticsRecord>(queue);
            if (!force && batch.Count < 1) return;

            try
            {
                var sb = new StringBuilder(batch.Count * 64);
                for (int i = 0; i < batch.Count; i++)
                {
                    sb.AppendLine(JsonUtility.ToJson(batch[i]));
                }
                File.AppendAllText(filePath, sb.ToString(), Encoding.UTF8);
                queue.Clear();  
                // Debug.Log($"[Analytics] Flushed {batch.Count} to {_filePath}");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Analytics] Flush failed: {e.Message} (will retry)");
            }
        }
    }
}
