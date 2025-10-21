using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using SaveLoad;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SaveLoadButtonUI : MonoBehaviour
    {
        public InputField inputField;
        public Dropdown dropdown;
        public Button saveButton;
        public Button loadButton;

        private string saveFolder => Application.persistentDataPath;

        private void Start()
        {
            RefreshDropdown();
        }

        public void SaveGame()
        {
            if (inputField == null)
            {
                Debug.LogError("InputField reference is missing!");
                return;
            }

            string saveName = inputField.text.Trim();
            Debug.Log($"[SaveLoadButtonUI] Save name entered: '{saveName}'");

            if (string.IsNullOrEmpty(saveName))
            {
                Debug.LogWarning("[SaveLoadButtonUI] Save name is empty. Aborting save.");
                return;
            }

            string filename = saveName;
            SaveLoad.SaveSystem.SaveToFile(GameManager.Instance.State, filename);

            RefreshDropdown();
        }




        public void LoadGame()
        {
            string selected = dropdown.options[dropdown.value].text;
            if (string.IsNullOrEmpty(selected) || selected == "No saves found") return;

            GameState loaded = SaveSystem.LoadFromFile(selected);
            if (loaded != null)
            {
                GameManager.Instance.State = loaded;
                Debug.Log($"Loaded save: {selected}");
            }
        }


        private void RefreshDropdown()
        {
            if (dropdown == null) return;

            string[] files = Directory.GetFiles(saveFolder, "save_*.json");
            List<string> names = files
                .Select(Path.GetFileNameWithoutExtension)
                .Select(name => name.Replace("save_", ""))
                .ToList();


            dropdown.ClearOptions();
            if (names.Count == 0)
            {
                names.Add("No saves found");
                dropdown.interactable = false;
            }
            else
            {
                dropdown.interactable = true;
            }

            dropdown.AddOptions(names);
        }
    }
}