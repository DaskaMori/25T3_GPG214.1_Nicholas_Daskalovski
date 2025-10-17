using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    public GameObject boxPrefab;
    public float spawnIntervalSeconds = 1.5f;
    public Vector3 spawnOffset = new Vector3(0f, 1f, 0f);

    // Assign these in the Inspector to your existing materials:
    // Box_Red, Box_Blue, Box_Green
    public Material materialRed;
    public Material materialBlue;
    public Material materialGreen;

    private float timer = 0f;

    private void Update()
    {
        timer = timer + Time.deltaTime;

        if (timer >= spawnIntervalSeconds)
        {
            timer = 0f;

            if (boxPrefab == null)
            {
                Debug.LogError("BoxSpawner: boxPrefab is NULL.");
                return;
            }

            Vector3 pos = transform.position + spawnOffset;
            GameObject g = Instantiate(boxPrefab, pos, Quaternion.identity);

            BoxData d = g.GetComponent<BoxData>();
            if (d == null)
            {
                Debug.LogError("BoxSpawner: Spawned box is missing BoxData.");
                return;
            }

            int roll = Random.Range(0, 3);
            if (roll == 0) { d.boxType = BoxType.Red; }
            if (roll == 1) { d.boxType = BoxType.Blue; }
            if (roll == 2) { d.boxType = BoxType.Green; }

            BoxTextureLoader loader = g.GetComponent<BoxTextureLoader>();
            if (loader != null)
            {
                loader.LoadTextureFromStreamingAssets();
            }

            // Helpful when debugging routes/splitters
            g.name = "Box_" + d.boxType.ToString();
        }
    }
}