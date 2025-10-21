using System.IO;
using Core;
using UnityEngine;

namespace Streaming
{
    public class BoxTextureLoader : MonoBehaviour
    {
        public void LoadTextureFromStreamingAssets()
        {
            BoxData data = GetComponent<BoxData>();
            if (data == null)
            {
                Debug.LogError("No BoxData component found on box.");
                return;
            }

            string fileName = GetFileNameForType(data.boxType);
            string filePath = Path.Combine(Application.streamingAssetsPath, "CrateSkins", fileName);
            filePath = filePath.Replace("\\", "/");

            Debug.Log("[BoxTextureLoader] Looking for texture at: " + filePath);

            if (!File.Exists(filePath))
            {
                Debug.LogError("Texture file not found at path: " + filePath);
                return;
            }

            byte[] imageBytes = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);
            ApplyTextureToRenderer(texture);
        }

        private string GetFileNameForType(BoxType type)
        {
            switch (type)
            {
                case BoxType.Red: return "texture_carbon.png";
                case BoxType.Blue: return "texture_metal.png";
                case BoxType.Green: return "texture_wood.png";
                default: return "texture_carbon.png";
            }
        }

        private void ApplyTextureToRenderer(Texture2D texture)
        {
            Renderer renderer = GetComponentInChildren<Renderer>();
            if (renderer == null)
            {
                Debug.LogError("No Renderer found on box.");
                return;
            }

            Material materialInstance = new Material(renderer.sharedMaterial);
            materialInstance.mainTexture = texture;
            renderer.material = materialInstance;
        }
    }
}