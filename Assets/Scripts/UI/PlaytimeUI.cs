using UnityEngine;
using UnityEngine.UI;
using Core;

namespace UI
{
    public class PlaytimeUI : MonoBehaviour
    {
        public Text label;                  
        public PlaytimeTracker tracker;     

        void Awake()
        {
            if (!tracker) tracker = PlaytimeTracker.Instance ?? FindObjectOfType<PlaytimeTracker>(true);
            if (!label)   label   = GetComponent<Text>(); 
        }

        void Update()
        {
            if (label && tracker)
                label.text = "Playtime: " + tracker.GetFormatted();
        }
    }
}