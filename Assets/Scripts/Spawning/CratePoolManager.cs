using System.Collections.Generic;
using UnityEngine;

namespace Spawning
{
    public class CratePoolManager : MonoBehaviour
    {
        public static CratePoolManager Instance { get; private set; }

        [System.Serializable]
        public class PooledCrateType
        {
            public string type;
            public GameObject prefab;
            public int prewarmCount = 10;
            public Queue<GameObject> pool = new Queue<GameObject>();
        }

        public List<PooledCrateType> crateTypes = new();

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (var crateType in crateTypes)
            {
                Prewarm(crateType);
            }
        }

        private void Prewarm(PooledCrateType crateType)
        {
            for (int i = 0; i < crateType.prewarmCount; i++)
            {
                GameObject crate = Instantiate(crateType.prefab);
                crate.SetActive(false);
                crateType.pool.Enqueue(crate);
            }
        }

        public bool HasCrateType(string type)
        {
            return crateTypes.Exists(c => c.type == type);
        }

        public void AddCrateType(string type, GameObject prefab, int prewarmCount = 5)
        {
            if (HasCrateType(type))
            {
                //Debug.Log($"[CratePoolManager] '{type}' crate type already exists in pool.");
                return;
            }

            var newType = new PooledCrateType
            {
                type = type,
                prefab = prefab,
                prewarmCount = prewarmCount,
                pool = new Queue<GameObject>()
            };

            Prewarm(newType);
            crateTypes.Add(newType);

           //Debug.Log($"[CratePoolManager] Added new pooled crate type: {type}");
        }

        public GameObject GetCrate(string type, Vector3 position)
        {
            var match = crateTypes.Find(c => c.type == type);
            if (match == null)
            {
                //Debug.LogError("[CratePoolManager] No crate type found for: " + type);
                return null;
            }

            GameObject crate;
            if (match.pool.Count == 0)
            {
                //Debug.LogWarning($"[CratePoolManager] Pool exhausted for {type}, instantiating extra.");
                crate = Instantiate(match.prefab);
            }
            else
            {
                crate = match.pool.Dequeue();
            }

            crate.transform.position = position;
            crate.transform.rotation = Quaternion.identity;
            crate.SetActive(true);
            return crate;
        }

        public void ReturnCrate(GameObject crate, string type)
        {
            var match = crateTypes.Find(c => c.type == type);
            if (match == null)
            {
                //Debug.LogWarning($"[CratePoolManager] Unknown crate type '{type}', destroying instead.");
                Destroy(crate);
                return;
            }

            crate.SetActive(false);
            match.pool.Enqueue(crate);
        }
    }
}
