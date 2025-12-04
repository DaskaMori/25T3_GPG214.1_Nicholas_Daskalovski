using System.Collections.Generic;
using UnityEngine;

namespace ProceduralGen
{
    public class ProceduralPoolManager : MonoBehaviour
    {
        [System.Serializable] public class PoolEntry
        {
            public GameObject prefab;
            public int preload = 0;
        }

        public List<PoolEntry> entries = new();

        readonly Dictionary<GameObject, Queue<GameObject>> pool = new();

        void Awake()
        {
            foreach (var e in entries)
            {
                if (e.prefab == null || e.preload <= 0) continue;
                var q = GetQ(e.prefab);
                for (int i = 0; i < e.preload; i++)
                {
                    var go = Instantiate(e.prefab);
                    go.SetActive(false);
                    q.Enqueue(go);
                }
            }
        }

        Queue<GameObject> GetQ(GameObject p)
        {
            if (!pool.TryGetValue(p, out var q)) { q = new Queue<GameObject>(); pool[p] = q; }
            return q;
        }

        public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent = null)
        {
            var q = GetQ(prefab);
            GameObject go = q.Count > 0 ? q.Dequeue() : Instantiate(prefab);
            if (parent) go.transform.SetParent(parent, false);
            go.transform.SetPositionAndRotation(pos, rot);
            go.SetActive(true);
            return go;
        }

        public void Return(GameObject prefab, GameObject instance)
        {
            instance.SetActive(false);
            instance.transform.SetParent(transform, false);
            GetQ(prefab).Enqueue(instance);
        }
    }
}