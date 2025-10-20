using System;
using UnityEngine;

public class BinTrigger : MonoBehaviour
{
    public BoxType acceptsType = BoxType.Red;
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
        BoxData data = other.GetComponent<BoxData>();
        if (data == null)
        {
            return;
        }

        bool isCorrect = data.boxType == acceptsType;

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
