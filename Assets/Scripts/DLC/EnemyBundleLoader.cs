using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace DLC
{
    public static class EnemyBundleLoader
    {
        private static readonly Dictionary<string, AssetBundle> cache = new();

        public static IEnumerator LoadEnemyEvents(EnemyBundleCatalog catalog, List<EnemyEventDef> sink)
        {
            if (catalog == null || catalog.entries == null) yield break;

            foreach (var e in catalog.entries)
            {
                var path = Path.Combine(Application.streamingAssetsPath, "Bundles", "Enemies", e.bundleName);
#if UNITY_EDITOR || UNITY_STANDALONE
                var uri = "file://" + path;
#else
                var uri = path; // Android
#endif
                AssetBundle bundle;
                if (!cache.TryGetValue(e.bundleName, out bundle))
                {
                    if (!File.Exists(path))
                    {
                        Debug.LogError($"[EnemyBundleLoader] Missing bundle: {path}");
                        continue;
                    }

                    using var req = UnityWebRequestAssetBundle.GetAssetBundle(uri);
                    yield return req.SendWebRequest();
                    if (req.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError($"[EnemyBundleLoader] Failed load {e.bundleName}: {req.error}");
                        continue;
                    }

                    bundle = DownloadHandlerAssetBundle.GetContent(req);
                    if (bundle == null)
                    {
                        Debug.LogError($"[EnemyBundleLoader] Null bundle: {e.bundleName}");
                        continue;
                    }
                    cache[e.bundleName] = bundle;
                }

                foreach (var assetName in e.eventAssetNames)
                {
                    var def = bundle.LoadAsset<EnemyEventDef>(assetName);
                    if (def != null) sink.Add(def);
                    else Debug.LogWarning($"[EnemyBundleLoader] Asset not found: {assetName} in {e.bundleName}");
                }
            }
        }

        public static void UnloadAll(bool unloadAllLoadedObjects = false)
        {
            foreach (var kv in cache) kv.Value.Unload(unloadAllLoadedObjects);
            cache.Clear();
        }
    }
}
