using UnityEngine;

[CreateAssetMenu(menuName = "DLC/Enemy Bundle Catalog")]
public class EnemyBundleCatalog : ScriptableObject
{
    [System.Serializable]
    public struct Entry
    {
        public string bundleName;       
        public string[] eventAssetNames;
    }

    public Entry[] entries;
}