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
        }

        public BinUI[] trackedBins;

        private void Update()
        {
            var state = GameManager.Instance.State;
            if (state == null) return;

            foreach (var binUI in trackedBins)
            {
                if (binUI == null || binUI.bin == null || binUI.uiText == null) continue;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"{binUI.binLabel} Bin");

                int totalCorrect = 0;
                int totalIncorrect = 0;

                foreach (string typeRaw in binUI.binLabel.Split(','))
                {
                    string type = typeRaw.Trim();
                    var stats = state.GetCounts(type);
                    if (stats == null) continue;

                    totalCorrect += stats.correct;
                    totalIncorrect += stats.incorrect;

                    sb.AppendLine($"{type}: ✔ {stats.correct} × {stats.incorrect}");
                }


                sb.AppendLine($"Total: ✔ {totalCorrect} × {totalIncorrect}");
                binUI.uiText.text = sb.ToString();
            }
        }
    }
}