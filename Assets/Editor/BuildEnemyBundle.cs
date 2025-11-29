using UnityEditor;

namespace Editor
{
    public static class BuildEnemyBundle
    {
        [MenuItem("Tools/Build Enemy Bundles")]
        public static void Build()
        {
            string output = "Assets/StreamingAssets/Bundles/Enemies";
            BuildPipeline.BuildAssetBundles(output, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
            AssetDatabase.Refresh();
            UnityEngine.Debug.Log("Built bundles to: " + output);
        }
    }
}