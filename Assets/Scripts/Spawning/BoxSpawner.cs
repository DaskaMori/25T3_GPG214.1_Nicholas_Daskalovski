using Core;
using Streaming;
using UnityEngine;

namespace Spawning
{
    public class BoxSpawner : MonoBehaviour
    {
        public GameObject boxPrefab;
        public float spawnIntervalSeconds = 1.5f;
        public Vector3 spawnOffset = new Vector3(0f, 1f, 0f);

        private float timer = 0f;

        private readonly string[] boxTypes = { "Wood", "Carbon", "Metal" };

        private void Update()
        {
            timer += Time.deltaTime;

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
                    //Debug.LogError("BoxSpawner: Spawned box is missing BoxData.");
                    return;
                }

                int roll = Random.Range(0, boxTypes.Length);
                string chosenType = boxTypes[roll];
                d.SetType(chosenType);

                BoxTextureLoader loader = g.GetComponent<BoxTextureLoader>();
                loader?.LoadTextureFromStreamingAssets();

                g.name = "Box_" + chosenType;
            }
        }
    }
}