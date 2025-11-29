using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SaveLoad;  

namespace UI
{
    public class SaveLoadButtonUI : MonoBehaviour
    {
        [Header("UI")]
        public InputField inputField;   
        public Dropdown dropdown;       
        public Button saveButton;
        public Button loadButton;

        [Header("Systems")]
        public SaveManager saveManager; 

        void Awake()
        {
            if (!saveManager) saveManager = FindObjectOfType<SaveManager>();

            if (saveButton) saveButton.onClick.AddListener(OnClickSave);
            if (loadButton) loadButton.onClick.AddListener(OnClickLoad);
        }

        void Start()
        {
            RefreshDropdown();
            SyncInputWithCurrentProfile();
        }


        void OnClickSave()
        {
            if (!saveManager) return;

            var typed = inputField ? inputField.text.Trim() : string.Empty;
            if (!string.IsNullOrEmpty(typed))
                saveManager.SetCurrentProfile(typed);

            saveManager.SaveNow();   

            RefreshDropdown();
            SelectDropdownCurrent();
            SyncInputWithCurrentProfile();
        }

        void OnClickLoad()
        {
            if (!saveManager || dropdown == null || dropdown.options.Count == 0) return;

            var selected = dropdown.options[dropdown.value].text;
            if (string.IsNullOrEmpty(selected) || selected == "No saves found") return;

            saveManager.LoadProfile(selected);  // switches and loads that profile

            RefreshDropdown();
            SelectDropdownCurrent();
            SyncInputWithCurrentProfile();
        }


        void RefreshDropdown()
        {
            if (dropdown == null || saveManager == null) return;

            var profiles = saveManager.ListProfiles(); // discovers "*_savegame.json"
            if (profiles.Count == 0)
            {
                dropdown.ClearOptions();
                dropdown.AddOptions(new List<string> { "No saves found" });
                dropdown.interactable = false;
                return;
            }

            dropdown.interactable = true;
            dropdown.ClearOptions();
            dropdown.AddOptions(profiles);

            SelectDropdownCurrent();
        }

        void SelectDropdownCurrent()
        {
            if (dropdown == null || saveManager == null || dropdown.options.Count == 0) return;

            var target = saveManager.currentProfileName;
            int idx = dropdown.options.FindIndex(o => o.text == target);
            dropdown.value = Mathf.Clamp(idx < 0 ? 0 : idx, 0, dropdown.options.Count - 1);
            dropdown.RefreshShownValue();
        }

        void SyncInputWithCurrentProfile()
        {
            if (inputField != null && saveManager != null)
                inputField.text = saveManager.currentProfileName;
        }
    }
}
