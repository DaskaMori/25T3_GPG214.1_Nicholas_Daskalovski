using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SaveLoad;

namespace UI
{
    public class SaveLoadHUD : MonoBehaviour
    {
        public InputField inputProfile;
        public Dropdown profilesDropdown;
        public Button btnSave;
        public Button btnLoad;

        public Text labelProfile;

        [Header("Systems")]
        public SaveManager saveManager;

        bool suppressUi;

        void Awake()
        {
            if (btnSave)           btnSave.onClick.AddListener(OnClickSave);
            if (btnLoad)           btnLoad.onClick.AddListener(OnClickLoad);

            if (inputProfile)
            {
                inputProfile.onValueChanged.AddListener(OnProfileTyping);
                inputProfile.onEndEdit.AddListener(OnProfileChanged);
            }

            if (profilesDropdown)
                profilesDropdown.onValueChanged.AddListener(_ => OnDropdownChanged());
        }

        void OnEnable()
        {
            if (!saveManager) saveManager = FindObjectOfType<SaveManager>(true);

            if (labelProfile == null)
                labelProfile = GetComponentsInChildren<Text>(true)
                                .FirstOrDefault(t => t.name == "Label_Profile");

            if (saveManager)
            {
                saveManager.OnSaved  += OnSavedOrLoaded;
                saveManager.OnLoaded += OnSavedOrLoaded;
            }

            PopulateProfiles();
            SyncProfileNameField();
        }

        void OnDisable()
        {
            if (saveManager)
            {
                saveManager.OnSaved  -= OnSavedOrLoaded;
                saveManager.OnLoaded -= OnSavedOrLoaded;
            }
        }

        void OnClickSave()
        {
            if (!saveManager) return;
            if (inputProfile && !string.IsNullOrWhiteSpace(inputProfile.text))
                saveManager.SetCurrentProfile(inputProfile.text);

            saveManager.SaveNow();
            PopulateProfiles();
            SelectDropdownCurrent();
        }

        void OnClickLoad()
        {
            if (!saveManager) return;
            saveManager.LoadNow();
            SyncProfileNameField();
            SelectDropdownCurrent();
        }

        /*void OnClickCreateOrSwitch()
        {
            if (!saveManager) return;

            var name = inputProfile && !string.IsNullOrWhiteSpace(inputProfile.text)
                ? inputProfile.text
                : GetDropdownSelectedProfile();

            if (string.IsNullOrWhiteSpace(name)) name = "Player";

            saveManager.LoadProfile(name);
            PopulateProfiles();
            SelectDropdownCurrent();
            SyncProfileNameField();
        }*/
        
        void SetProfileLabel(string value)
        {
            if (labelProfile) labelProfile.text = value ?? "";
        }


        void OnDropdownChanged()
        {
            if (suppressUi) return;
            if (!saveManager || profilesDropdown == null || profilesDropdown.options.Count == 0) return;

            var selected = GetDropdownSelectedProfile();
            if (string.IsNullOrWhiteSpace(selected)) return;

            saveManager.LoadProfile(selected);
            SyncProfileNameField();
            SetProfileLabel(saveManager.currentProfileName);   // NEW: reflect dropdown selection
        }


        void OnProfileTyping(string value)
        {
            if (suppressUi) return;
            saveManager?.SetCurrentProfile(value);
            SelectDropdownCurrent();
            SetProfileLabel(value);                            
        }

        void OnProfileChanged(string value)
        {
            if (suppressUi) return;
            saveManager?.SetCurrentProfile(value);
            SelectDropdownCurrent();
            SetProfileLabel(value);                            
        }


        void OnSavedOrLoaded(SaveData _)
        {
            SyncProfileNameField();
            PopulateProfiles();
            SelectDropdownCurrent();
        }

        void PopulateProfiles()
        {
            if (profilesDropdown == null || saveManager == null) return;

            suppressUi = true;
            var profiles = saveManager.ListProfiles();
            if (!string.IsNullOrWhiteSpace(saveManager.currentProfileName) &&
                !profiles.Contains(saveManager.currentProfileName))
            {
                profiles.Add(saveManager.currentProfileName);
            }

            profilesDropdown.ClearOptions();
            profilesDropdown.AddOptions(new List<string>(profiles));
            SelectDropdownCurrent();
            suppressUi = false;
        }

        void SelectDropdownCurrent()
        {
            if (profilesDropdown == null || saveManager == null || profilesDropdown.options.Count == 0) return;

            var target = saveManager.currentProfileName;
            int idx = profilesDropdown.options.FindIndex(o => o.text == target);
            if (idx < 0) idx = 0;

            profilesDropdown.SetValueWithoutNotify(idx);
            profilesDropdown.RefreshShownValue();
        }

        string GetDropdownSelectedProfile()
        {
            if (profilesDropdown == null || profilesDropdown.options.Count == 0) return null;
            return profilesDropdown.options[profilesDropdown.value].text;
        }

        void SyncProfileNameField()
        {
            if (!inputProfile || saveManager == null) return;

            suppressUi = true;
            inputProfile.SetTextWithoutNotify(saveManager.currentProfileName);
            suppressUi = false;

            if (labelProfile) labelProfile.text = saveManager.currentProfileName;
        }
    }
}
