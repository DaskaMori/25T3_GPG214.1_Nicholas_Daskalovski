using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(AudioSource))]
public class StreamingAudioManager : MonoBehaviour
{
    public AudioSource conveyorLoopSource;
    public AudioSource correctDingSource;
    public AudioSource incorrectBuzzSource;

    public string conveyorFile = "conveyor_loop.wav";
    public string correctFile = "correct_ding.wav";
    public string incorrectFile = "incorrect_buzz.wav";

    private void Start()
    {
        LoadAndAssign(conveyorFile, conveyorLoopSource, true);
        LoadAndAssign(correctFile, correctDingSource, false);
        LoadAndAssign(incorrectFile, incorrectBuzzSource, false);
    }

    private void LoadAndAssign(string fileName, AudioSource targetSource, bool playOnLoad)
    {
        StartCoroutine(LoadAudioClipCoroutine(fileName, targetSource, playOnLoad));
    }

    private IEnumerator LoadAudioClipCoroutine(string fileName, AudioSource targetSource, bool playOnLoad)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "SFX", fileName);
        filePath = filePath.Replace("\\", "/");

#if UNITY_EDITOR || UNITY_STANDALONE
        string url = "file:///" + filePath;
#elif UNITY_ANDROID
        string url = filePath;
#else
        string url = "file://" + filePath;
#endif

        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load audio file: " + fileName + " | Error: " + request.error);
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
            targetSource.clip = clip;

            if (playOnLoad)
            {
                targetSource.Play();
            }
        }
    }

    public void PlayCorrectSound()
    {
        correctDingSource?.Play();
    }

    public void PlayIncorrectSound()
    {
        incorrectBuzzSource?.Play();
    }
}
