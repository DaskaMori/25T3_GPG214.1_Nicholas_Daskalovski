using Core;
using UnityEngine;
using UnityEngine.UI;
using SaveLoad;

namespace UI
{
    public class BinStatsUiManager : MonoBehaviour
    {
        public BinTrigger redBin;
        public BinTrigger greenBin;
        public BinTrigger blueBin;

        public Text redText;
        public Text greenText;
        public Text blueText;

        private void Update()
        {
            var state = GameManager.Instance.State;

            var red = state.GetCounts("Red");
            var green = state.GetCounts("Green");
            var blue = state.GetCounts("Blue");

            redText.text = $"Red Bin\nCorrect: {red.correct}\nIncorrect: {red.incorrect}";
            greenText.text = $"Green Bin\nCorrect: {green.correct}\nIncorrect: {green.incorrect}";
            blueText.text = $"Blue Bin\nCorrect: {blue.correct}\nIncorrect: {blue.incorrect}";
        }


    }
}

