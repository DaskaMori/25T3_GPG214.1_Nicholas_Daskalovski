using System;
using Streaming;
using UnityEngine;

namespace Core
{
    public class BinTrigger : MonoBehaviour
    {
        public BoxType acceptsType;
        public int correctCount = 0;
        public int incorrectCount = 0;

        private StreamingAudioManager sfxManager;

        [Obsolete("Obsolete")]
        private void Start()
        {
            sfxManager = FindObjectOfType<StreamingAudioManager>();
            if (sfxManager == null)
            {
                Debug.LogWarning("BinTrigger: No StreamingAudioManager found in scene.");
            }
        }
    
        private void OnTriggerEnter(Collider other)
        {
            BoxData box = other.GetComponent<BoxData>();
            if (box == null) return;

            bool isCorrect = box.boxType == acceptsType;

            GameManager.Instance.RecordSort(acceptsType.ToString(), isCorrect);

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
