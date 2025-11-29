using UnityEngine;
using UnityEngine.UI;
using SaveLoad;

namespace UI
{
    public class BottomStatsHUD : MonoBehaviour
    {
        [Header("UI")]
        public Text correctLabel;
        public Text incorrectLabel;
        public Text hazardsLabel;

        [Header("Systems (optional – will auto-find)")]
        public SaveManager saveManager;
        public Core.GameManager gameManager;
        public DLC.EnemyEventManager enemyManager;

        float tick;

        void Awake()
        {
            if (!saveManager)   saveManager = FindObjectOfType<SaveLoad.SaveManager>();
            if (!gameManager)   gameManager = FindObjectOfType<Core.GameManager>();
            if (!enemyManager)  enemyManager = FindObjectOfType<DLC.EnemyEventManager>();
        }

        void OnEnable()
        {
            if (!saveManager) saveManager = FindObjectOfType<SaveManager>();
            if (!gameManager) gameManager = FindObjectOfType<Core.GameManager>();
            if (!enemyManager) enemyManager = FindObjectOfType<DLC.EnemyEventManager>();

            if (saveManager != null)
            {
                saveManager.OnSaved  += HandleSaveOrLoad;
                saveManager.OnLoaded += HandleSaveOrLoad;  
            }

            RefreshLive();
        }

        void OnDisable()
        {
            if (saveManager != null)
            {
                saveManager.OnSaved  -= HandleSaveOrLoad;
                saveManager.OnLoaded -= HandleSaveOrLoad;  
            }
        }

        void HandleSaveOrLoad(SaveData _)
        {
            RefreshLive();
        }


        void Update()
        {
            tick += Time.unscaledDeltaTime;
            if (tick >= 0.5f)
            {
                tick = 0f;
                RefreshLive();
            }
        }

        void RefreshLive()
        {
            if (!correctLabel || !incorrectLabel || !hazardsLabel) return;
            if (!gameManager || !enemyManager) return;

            int correct   = gameManager.GetTotalCorrectlySorted();
            int incorrect = gameManager.GetTotalIncorrectlySorted();   
            int averted   = enemyManager.GetHazardsAvertedLifetime();  

            correctLabel.text   = $"✔ {correct}";
            incorrectLabel.text = $"✖ {incorrect}";
            hazardsLabel.text   = $"🛡 {averted}";
        }
    }
}