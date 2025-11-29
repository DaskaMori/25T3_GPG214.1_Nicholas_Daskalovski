using System.Collections.Generic;
using UnityEngine;
using Spawning;
using Streaming;

namespace Core
{
    public class BinTrigger : MonoBehaviour
    {
        [Header("Accepted crate types (case-insensitive)")]
        public string[] acceptedTypes;

        [Header("Local counters (optional, for per-bin UI)")]
        public int correctCount = 0;
        public int incorrectCount = 0;

        private StreamingAudioManager sfxManager;
        private HashSet<string> acceptedSet; 

        private void Awake()
        {
            acceptedSet = new HashSet<string>();
            if (acceptedTypes != null)
            {
                for (int i = 0; i < acceptedTypes.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(acceptedTypes[i]))
                        acceptedSet.Add(acceptedTypes[i].Trim().ToLowerInvariant());
                }
            }
        }

        private void Start()
        {
            sfxManager = FindObjectOfType<StreamingAudioManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            var box = other.GetComponent<BoxData>();
            if (box == null) return;

            string boxType = (box.boxType ?? string.Empty).Trim().ToLowerInvariant();
            bool isCorrect = acceptedSet != null && acceptedSet.Contains(boxType);

            GameManager.Instance.RecordSort(isCorrect);

            if (isCorrect)
            {
                correctCount++;
                sfxManager?.PlayCorrectSound();
            }
            else
            {
                incorrectCount++;
                sfxManager?.PlayIncorrectSound();
            }
            
            Analytics.AnalyticsManager.Instance?.LogSort(box.boxType, isCorrect);
            if (!isCorrect)
            {
                var targetBinName = name; 
                Analytics.AnalyticsManager.Instance?.LogMistake(box.boxType, targetBinName);
            }

            CratePoolManager.Instance.ReturnCrate(other.gameObject, box.boxType);
        }
    }
}