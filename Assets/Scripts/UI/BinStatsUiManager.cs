/*using Core;
using UnityEngine;
using UnityEngine.UI;
using SaveLoad;

namespace UI
{
    public class BinStatsUiManager : MonoBehaviour
    {
        public BinTrigger woodBin;
        public BinTrigger carbonBin;
        public BinTrigger metalBin;

        public Text woodText;
        public Text carbonText;
        public Text metalText;

        private void Update()
        {
            var state = GameManager.Instance.State;

            var red = state.GetCounts("Wood");
            var green = state.GetCounts("Carbon");
            var blue = state.GetCounts("Metal");

            woodText.text = $"Wood Bin\nCorrect: {red.correct}\nIncorrect: {red.incorrect}";
            carbonText.text = $"Carbon Bin\nCorrect: {green.correct}\nIncorrect: {green.incorrect}";
            metalText.text = $"Metal Bin\nCorrect: {blue.correct}\nIncorrect: {blue.incorrect}";
        }


    }
}*/

