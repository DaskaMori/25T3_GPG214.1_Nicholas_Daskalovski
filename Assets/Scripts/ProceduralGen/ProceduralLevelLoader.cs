using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ProceduralGen
{
    public class ProceduralLevelLoader : MonoBehaviour
    {
        [Header("Level Texture")]
        public string levelFileName = "Level01.png";

        [Header("Colour → Prefab Map")]
        public List<ColorPrefabPair> mappings = new List<ColorPrefabPair>();

        [Header("Grid Settings")]
        public float cellSize = 1f;
        public Vector3 origin = Vector3.zero;
        public Transform staticParent;               
        public bool markStatic = true;                

        [Header("Matching")]
        [Range(0, 8)] public int tolerance = 0; 
        public bool logTexturePaletteOnce = true;

        [Header("Pooling / Batching")]
        public ProceduralPoolManager pool;                      
        [Range(16, 1024)] public int batchSize = 128;  
        public bool yieldDuringBuild = true;          

        private Dictionary<int, MapEntry> map;       
        private HashSet<int> unknownOnce; 
        private List<SpawnRequest> spawnQueue;       
        private Color32[] pixelsCache;        

        private struct SpawnRequest
        {
            public GameObject prefab;
            public Vector3 pos;
            public Quaternion rot;
            public Transform parent;
        }

        private struct MapEntry
        {
            public GameObject prefab;
            public Vector3 offset;
            public bool usePool;
        }

        private void Start()
        {
            BuildDictionary();
            StartCoroutine(LoadLevelOptimised());
        }

        private static int RGBKey(Color32 c) => (c.r << 16) | (c.g << 8) | c.b;

        private void BuildDictionary()
        {
            map = new Dictionary<int, MapEntry>(mappings.Count);
            unknownOnce = new HashSet<int>();
            spawnQueue = new List<SpawnRequest>(4096);

            foreach (var m in mappings)
            {
                var c32 = (Color32)m.color;
                int key = RGBKey(c32);

                if (map.ContainsKey(key))
                    continue;

                if (m.prefab == null)
                    Debug.LogWarning($"[ProceduralLevelLoader] Mapping for RGB({c32.r},{c32.g},{c32.b}) has NO prefab assigned.");

                map.Add(key, new MapEntry
                {
                    prefab = m.prefab,
                    offset = m.offset,
                    usePool = m.usePool
                });
            }

            Debug.Log($"[ProceduralLevelLoader] Registered {map.Count} colour mappings.");
        }

        private bool TryGetEntry(Color32 c, out MapEntry entry)
        {
            if (tolerance == 0)
                return map.TryGetValue(RGBKey(c), out entry);

            foreach (var kv in map)
            {
                byte r = (byte)((kv.Key >> 16) & 0xFF);
                byte g = (byte)((kv.Key >> 8) & 0xFF);
                byte b = (byte)(kv.Key & 0xFF);

                if (Mathf.Abs(r - c.r) <= tolerance &&
                    Mathf.Abs(g - c.g) <= tolerance &&
                    Mathf.Abs(b - c.b) <= tolerance)
                {
                    entry = kv.Value;
                    return true;
                }
            }

            entry = default;
            return false;
        }

        private IEnumerator LoadLevelOptimised()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "LevelMaps", levelFileName);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[ProceduralLevelLoader] Missing texture: {path}");
                yield break;
            }

            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false, false);
            tex.LoadImage(File.ReadAllBytes(path));

            pixelsCache = tex.GetPixels32();
            int w = tex.width, h = tex.height;

            if (logTexturePaletteOnce)
            {
                LogUniquePalette(pixelsCache);
                logTexturePaletteOnce = false;
            }

            spawnQueue.Clear();
            int placedImmediate = 0; 

            for (int y = 0; y < h; y++)
            {
                int row = y * w;
                for (int x = 0; x < w; x++)
                {
                    var c = pixelsCache[row + x];
                    if (c.a == 0) continue;

                    if (TryGetEntry(c, out var entry) && entry.prefab != null)
                    {
                        Vector3 pos = origin + new Vector3(x * cellSize, 0f, y * cellSize) + entry.offset;

                        if (entry.usePool && pool != null)
                        {
                            var go = pool.Get(entry.prefab, pos, Quaternion.identity, staticParent);
                            if (markStatic && go) go.isStatic = true;
                            placedImmediate++;
                        }
                        else
                        {
                            spawnQueue.Add(new SpawnRequest
                            {
                                prefab = entry.prefab,
                                pos = pos,
                                rot = Quaternion.identity,
                                parent = staticParent
                            });
                        }
                    }
                    else
                    {
                        int key = RGBKey(c);
                        if (unknownOnce.Add(key))
                            Debug.LogWarning($"[ProceduralLevelLoader] Unknown colour RGB({c.r},{c.g},{c.b}) (first seen at {x},{y})");
                    }
                }
            }

            Destroy(tex);
            pixelsCache = null;

            int totalQueued = spawnQueue.Count;
            int spawned = 0;
            while (spawned < totalQueued)
            {
                int to = Mathf.Min(spawned + batchSize, totalQueued);
                for (int i = spawned; i < to; i++)
                {
                    var rq = spawnQueue[i];
                    var go = Instantiate(rq.prefab, rq.pos, rq.rot, rq.parent);
                    if (markStatic && go) go.isStatic = true;
                }
                spawned = to;

                if (yieldDuringBuild)
                    yield return null; 
            }

            spawnQueue.Clear();

            Debug.Log($"[ProceduralLevelLoader] Generation complete. Pooled={placedImmediate}, Instantiated={totalQueued}, Total={(placedImmediate + totalQueued)}");
        }

        private void LogUniquePalette(Color32[] pixels)
        {
            var set = new HashSet<int>();
            foreach (var c in pixels)
            {
                if (c.a == 0) continue;
                set.Add(RGBKey(c));
            }

            Debug.Log($"[ProceduralLevelLoader] Texture unique (RGB) colours = {set.Count}");
#if UNITY_EDITOR
            foreach (var key in set)
            {
                byte r = (byte)((key >> 16) & 0xFF);
                byte g = (byte)((key >> 8) & 0xFF);
                byte b = (byte)(key & 0xFF);
                Debug.Log($" • RGB({r},{g},{b})");
            }
#endif
        }
    }

    [System.Serializable]
    public class ColorPrefabPair
    {
        public Color color;
        public GameObject prefab;
        public Vector3 offset;

        [Tooltip("If true and a SimplePool is assigned to the loader, this entry uses pooled reuse instead of Instantiate.")]
        public bool usePool = true;
    }
    
    public class SimplePool : MonoBehaviour
    {
        // per-prefab stacks
        private readonly Dictionary<GameObject, Stack<GameObject>> pool = new();

        /// <summary>
        /// Get an instance (reused or new). Parent is optional.
        /// </summary>
        public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent = null)
        {
            if (prefab == null) return null;

            if (!pool.TryGetValue(prefab, out var stack) || stack.Count == 0)
            {
                var go = Instantiate(prefab, pos, rot, parent);
                go.SetActive(true);
                return go;
            }

            var re = stack.Pop();
            if (parent) re.transform.SetParent(parent, worldPositionStays: false);
            re.transform.SetPositionAndRotation(pos, rot);
            re.SetActive(true);
            return re;
        }

        
        public void Return(GameObject prefab, GameObject instance)
        {
            if (prefab == null || instance == null) return;

            if (!pool.TryGetValue(prefab, out var stack))
            {
                stack = new Stack<GameObject>(32);
                pool[prefab] = stack;
            }

            instance.SetActive(false);
            instance.transform.SetParent(transform, false);
            stack.Push(instance);
        }
    }
}
