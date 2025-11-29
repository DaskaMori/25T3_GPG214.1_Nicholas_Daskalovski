using System;
using UnityEngine;
using UnityEngine.UI;

namespace DLC
{
    public class EnemyEventUI : MonoBehaviour
    {
        [Header("Display")]
        public Image icon;
        public Text title;
        public Text timer;

        [Header("Action Menu")]
        public Button menuToggle;     
        public GameObject menuPanel;  
        public Button actionA;        
        public Button actionB;        
        public Button actionC;        

        public void Setup(Sprite s, string t)
        {
            if (icon)  icon.sprite = s;
            if (title) title.text = t;
            if (menuPanel) menuPanel.SetActive(false);
            if (menuToggle)
                menuToggle.onClick.AddListener(() => menuPanel?.SetActive(!(menuPanel && menuPanel.activeSelf)));
        }

        public void SetTime(float t)
        {
            if (timer) timer.text = $"{t:0}s";
        }

        public void BindActions(Action onA, Action onB, Action onC)
        {
            if (actionA) { actionA.onClick.RemoveAllListeners(); actionA.onClick.AddListener(() => onA?.Invoke()); }
            if (actionB) { actionB.onClick.RemoveAllListeners(); actionB.onClick.AddListener(() => onB?.Invoke()); }
            if (actionC) { actionC.onClick.RemoveAllListeners(); actionC.onClick.AddListener(() => onC?.Invoke()); }
        }
    }
}