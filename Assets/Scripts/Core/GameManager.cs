using SaveLoad;
using UnityEngine;
using UI;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; set; }

        public GameState State { get; set; } = new GameState();

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

        public void SaveGame(string saveName)
        {
            SaveSystem.SaveToFile(State, saveName);
        }

        public void LoadGame(string saveName)
        {
            GameState loaded = SaveSystem.LoadFromFile(saveName);
            if (loaded != null)
            {
                State = loaded;
                Debug.Log($"[GameManager] Loaded: {saveName}");
            }
            else
            {
                Debug.LogWarning($"[GameManager] No save found for {saveName}, loading defaults.");
                State = new GameState();
            }
        }


        public void RecordSort(string binColor, bool isCorrect)
        {
            State.Increment(binColor, isCorrect);
        }



    }
}