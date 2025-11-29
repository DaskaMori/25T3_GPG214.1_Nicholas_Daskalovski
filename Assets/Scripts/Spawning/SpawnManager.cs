using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Spawning
{
    public static class SpawnManager
    {
        private static readonly Dictionary<string, AssetBundle> loadedBundles = new();

        public static IEnumerator SpawnCrateAsync(string bundleName, string assetName, Vector3 position)
        {
            AssetBundle bundle = null;

            if (!loadedBundles.TryGetValue(bundleName, out bundle))
            {
                string path = Path.Combine(Application.streamingAssetsPath, "Bundles", "Crates", bundleName);
                string uri = "file://" + path;

                UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri);
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    //Debug.LogError($"[SpawnManager] Failed to load bundle '{bundleName}': {request.error}");
                    yield break;
                }

                bundle = DownloadHandlerAssetBundle.GetContent(request);
                if (bundle == null)
                {
                    //Debug.LogError($"[SpawnManager] Bundle '{bundleName}' is null.");
                    yield break;
                }

                loadedBundles[bundleName] = bundle;
                //Debug.Log($"[SpawnManager] Cached AssetBundle '{bundleName}'.");
            }

            GameObject prefab = bundle.LoadAsset<GameObject>(assetName);
            if (prefab == null)
            {
                //Debug.LogError($"[SpawnManager] Crate prefab '{assetName}' not found in '{bundleName}'.");
                yield break;
            }
            
            if (!CratePoolManager.Instance.HasCrateType(assetName))
            {
                CratePoolManager.Instance.AddCrateType(assetName, prefab, 5);
                //Debug.Log($"[SpawnManager] Added '{assetName}' crate to pool (from {bundleName}).");
            }

            GameObject pooledCrate = CratePoolManager.Instance.GetCrate(assetName, position);
            if (pooledCrate == null)
            {
                //Debug.LogError($"[SpawnManager] Failed to spawn pooled crate: {assetName}");
                yield break;
            }

            pooledCrate.name = "Box_" + assetName;
        }
    }
}
