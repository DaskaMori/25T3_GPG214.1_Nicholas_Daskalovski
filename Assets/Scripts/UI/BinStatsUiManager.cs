using Core;
using UnityEngine;
using UnityEngine.UI;
using SaveLoad;
using System.Text;

namespace UI
{
    public class BinStatsUiManager : MonoBehaviour
    {
        [System.Serializable]
        public class BinUI
        {
            public string binLabel;      
            public BinTrigger bin;       
            public Text uiText;          

            [HideInInspector] 
            public string[] acceptedTypes;  // Pre-split once
        }

        public BinUI[] trackedBins;

        // Reusable StringBuilder — no per-frame GC
        private readonly StringBuilder sb = new StringBuilder(128);

        private void Awake()
        {
            // Pre-split all bin labels so we don’t call Split() every frame
            foreach (var binUI in trackedBins)
            {
                if (binUI == null || string.IsNullOrEmpty(binUI.binLabel)) continue;
                binUI.acceptedTypes = binUI.binLabel.Split(',');
                for (int i = 0; i < binUI.acceptedTypes.Length; i++)
                    binUI.acceptedTypes[i] = binUI.acceptedTypes[i].Trim();
            }
        }

        private void Update()
        {
            var state = GameManager.Instance.State;
            if (state == null) return;

            foreach (var binUI in trackedBins)
            {
                if (binUI == null || binUI.bin == null || binUI.uiText == null) continue;

                sb.Clear();
                sb.Append(binUI.binLabel).Append(" Bin\n");

                int totalCorrect = 0;
                int totalIncorrect = 0;

                // Use pre-split array (no new allocations)
                var types = binUI.acceptedTypes;
                if (types == null) continue;

                for (int i = 0; i < types.Length; i++)
                {
                    var stats = state.GetCounts(types[i]);
                    if (stats == null) continue;

                    totalCorrect += stats.correct;
                    totalIncorrect += stats.incorrect;
                    sb.Append(types[i]).Append(": ✔ ").Append(stats.correct)
                      .Append(" × ").Append(stats.incorrect).Append('\n');
                }

                sb.Append("Total: ✔ ").Append(totalCorrect)
                  .Append(" × ").Append(totalIncorrect);

                // Assign the single built string (minimal GC)
                binUI.uiText.text = sb.ToString();
            }
        }
    }
}
