using UnityEngine;

namespace Core
{
    public class PlaytimeTracker : MonoBehaviour
    {
        public static PlaytimeTracker Instance { get; private set; }
        
        private long baseSeconds = 0;        
        private double sessionSeconds = 0.0; 

        private bool isPaused = false;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            if (isPaused) return;
            sessionSeconds += Time.unscaledDeltaTime;
        }

        public void SetPaused(bool paused) => isPaused = paused;

        public void ApplyLoadedBase(long loadedTotalSeconds)
        {
            baseSeconds = Mathf.Max(0, (int)loadedTotalSeconds);
            sessionSeconds = 0.0; 
        }

        public long GetTotalSeconds() => baseSeconds + (long)sessionSeconds;

        public string GetFormatted() => Format(GetTotalSeconds());

        public static string Format(long totalSeconds)
        {
            long h = totalSeconds / 3600;
            long m = (totalSeconds % 3600) / 60;
            long s = totalSeconds % 60;
            return $"{h:00}:{m:00}:{s:00}";
        }
    }
}