using UnityEngine;

public enum BoxType
{
    Red,
    Blue,
    Green
}

public class BoxData : MonoBehaviour
{
    public BoxType boxType = BoxType.Red;
    public float weight = 1f;

    // Call this after setting boxType
    public void ApplyMaterial(Material red, Material blue, Material green)
    {
        if (red == null || blue == null || green == null)
        {
            Debug.LogError("BoxData.ApplyMaterial: One or more material references are NULL.");
            return;
        }

        Material chosen = red;
        if (boxType == BoxType.Blue) { chosen = blue; }
        if (boxType == BoxType.Green) { chosen = green; }

        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        if (renderers == null || renderers.Length == 0)
        {
            Debug.LogError("BoxData.ApplyMaterial: No Renderer found on box prefab or its children.");
            return;
        }

        // Apply to every renderer and every submesh
        for (int i = 0; i < renderers.Length; i = i + 1)
        {
            Renderer r = renderers[i];
            if (r == null) { continue; }

            Material[] mats = r.sharedMaterials; 
            if (mats == null || mats.Length == 0)
            {
                r.sharedMaterial = chosen;
                continue;
            }

            for (int m = 0; m < mats.Length; m = m + 1)
            {
                mats[m] = chosen;
            }
            r.sharedMaterials = mats;
        }
    }
}