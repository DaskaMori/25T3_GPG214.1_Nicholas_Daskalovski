using Spawning;
using UnityEngine;

namespace Core
{
    public class FloorReturnTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            BoxData box = other.GetComponent<BoxData>();
            if (box == null) return;   

            GameManager.Instance.RecordSort(false); 
            
            CratePoolManager.Instance.ReturnCrate(other.gameObject, box.boxType);
        }
    }
}