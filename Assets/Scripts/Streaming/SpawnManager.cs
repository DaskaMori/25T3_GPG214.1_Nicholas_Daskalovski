using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Streaming
{
    public static class SpawnManager
    {
        public static IEnumerator SpawnCrateAsync(string bundleName, string assetName, Vector3 position)
        {
            string path = System.IO.Path.Combine(Application.streamingAssetsPath, "Bundles", "Crates", bundleName);
            string uri = "file://" + path;

            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[SpawnManager] Failed to load bundle '{bundleName}': {request.error}");
                yield break;
            }

            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
            if (bundle == null)
            {
                Debug.LogError($"[SpawnManager] Bundle is null: {bundleName}");
                yield break;
            }

            AssetBundleRequest assetLoad = bundle.LoadAssetAsync<GameObject>(assetName);
            yield return assetLoad;

            GameObject prefab = assetLoad.asset as GameObject;
            if (prefab == null)
            {
                Debug.LogError($"[SpawnManager] Crate prefab '{assetName}' not found in bundle '{bundleName}'");
                bundle.Unload(false);
                yield break;
            }

            GameObject spawned = GameObject.Instantiate(prefab, position, Quaternion.identity);
            spawned.name = "Box_" + assetName;

            bundle.Unload(false); // Keep assets in memory, unload container
        }
    }
}