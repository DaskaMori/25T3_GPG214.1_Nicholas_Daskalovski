using System.IO;
using Core;
using UnityEngine;

namespace Streaming
{
    public class BoxTextureLoader : MonoBehaviour
    {
        public string textureFolder = "CrateSkins"; 

        public void LoadTextureFromStreamingAssets()
        {
            BoxData data = GetComponent<BoxData>();
            if (data == null)
            {
                //Debug.LogError("[BoxTextureLoader] No BoxData component found on this object.");
                return;
            }

            string fileName = $"texture_{data.boxType.ToLower()}.png";
            string filePath = Path.Combine(Application.streamingAssetsPath, textureFolder, fileName);
            filePath = filePath.Replace("\\", "/");

            //Debug.Log($"[BoxTextureLoader] Looking for texture at: {filePath}");

            if (!File.Exists(filePath))
            {
                //Debug.LogWarning($"[BoxTextureLoader] Texture not found for '{data.boxType}', using fallback.");
                ApplyFallbackTexture();
                return;
            }

            byte[] imageBytes = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);
            ApplyTextureToRenderer(texture);
        }

        private void ApplyTextureToRenderer(Texture2D texture)
        {
            Renderer renderer = GetComponentInChildren<Renderer>();
            if (renderer == null)
            {
                //Debug.LogError("[BoxTextureLoader] No Renderer found on prefab.");
                return;
            }

            Material materialInstance = new Material(renderer.sharedMaterial);
            materialInstance.mainTexture = texture;
            renderer.material = materialInstance;

            //Debug.Log("[BoxTextureLoader] Texture applied successfully!");
        }

        private void ApplyFallbackTexture()
        {
            Renderer renderer = GetComponentInChildren<Renderer>();
            if (renderer == null) return;

            Material fallbackMat = new Material(renderer.sharedMaterial);
            fallbackMat.color = Color.gray;
            renderer.material = fallbackMat;
        }
    }
}
