using UnityEngine;


namespace Core
{
    [System.Serializable]
    public class BoxData : MonoBehaviour
    {
        [Header("Crate Properties")]
        public string boxType = "Red"; 
        public float weight = 1f;


        [Header("Optional Base Materials")]
        public Material materialRed;
        public Material materialBlue;
        public Material materialGreen;


        public void ApplyMaterial()
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
            if (renderers == null || renderers.Length == 0) return;


            Material chosen = null;
            switch (boxType)
            {
                case "Red": chosen = materialRed; break;
                case "Blue": chosen = materialBlue; break;
                case "Green": chosen = materialGreen; break;
                default: chosen = materialRed; break; 
            }


            foreach (var r in renderers)
                r.sharedMaterial = chosen;
        }


        public void SetType(string newType)
        {
            boxType = newType;
        }
    }
}