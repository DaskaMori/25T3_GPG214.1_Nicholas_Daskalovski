using UnityEngine;
using Core.Conveyor;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        private int liveCorrect = 0;
        private int liveIncorrect = 0;
        
        private int? loadedCorrect = null;
        private int? loadedIncorrect = null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void RecordSort(bool isCorrect)
        {
            if (isCorrect) liveCorrect++;
            else           liveIncorrect++;

            if (loadedCorrect.HasValue && loadedIncorrect.HasValue)
            {
                if (isCorrect) loadedCorrect++;
                else           loadedIncorrect++;
            }
        }
        
        public int GetTotalCorrectlySorted()  => loadedCorrect   ?? liveCorrect;
        public int GetTotalIncorrectlySorted()=> loadedIncorrect ?? liveIncorrect;

        
        public void ApplyLoadedTotals(int correct, int incorrect)
        {
            loadedCorrect = correct;
            loadedIncorrect = incorrect;

            Debug.Log($"[GameManager] Loaded totals → correct={correct}, incorrect={incorrect}");
        }
        
        public void ClearLoadedTotals()
        {
            loadedCorrect = null;
            loadedIncorrect = null;
        }
    }
}
