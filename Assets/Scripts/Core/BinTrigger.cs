using Streaming;
using UnityEngine;

namespace Core
{
    public class BinTrigger : MonoBehaviour
    {
        public string[] acceptedTypes;

        public int correctCount = 0;
        public int incorrectCount = 0;

        private StreamingAudioManager sfxManager;

        [System.Obsolete]
        private void Start()
        {
            sfxManager = FindObjectOfType<StreamingAudioManager>();
            if (sfxManager == null)
            {
                Debug.LogWarning("BinTrigger: No StreamingAudioManager found.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            BoxData box = other.GetComponent<BoxData>();
            if (box == null) return;

            string boxType = box.boxType;

            bool isCorrect = false;
            foreach (string accepted in acceptedTypes)
            {
                if (boxType.Equals(accepted, System.StringComparison.OrdinalIgnoreCase))
                {
                    isCorrect = true;
                    break;
                }
            }

            GameManager.Instance.RecordSort(boxType, isCorrect);

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

            Destroy(other.gameObject);
        }
    }
}