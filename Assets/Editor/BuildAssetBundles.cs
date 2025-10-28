using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class BuildAssetBundles
    {
        [MenuItem("Tools/Build Crate Asset Bundles")]
        public static void BuildAllCrateBundles()
        {
            string bundleDirectory = "Assets/StreamingAssets/Bundles/Crates";

            if (!Directory.Exists(bundleDirectory))
            {
                Directory.CreateDirectory(bundleDirectory);
                Debug.Log("[BuildAssetBundles] Created directory: " + bundleDirectory);
            }

            BuildPipeline.BuildAssetBundles(
                bundleDirectory,
                BuildAssetBundleOptions.None,
                BuildTarget.StandaloneWindows64
            );

            Debug.Log("[BuildAssetBundles] Crate bundles built to: " + bundleDirectory);
        }
    }
}