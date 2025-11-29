using System.Collections.Generic;
using Core;
using Streaming;
using UnityEngine;

namespace Spawning
{
    [System.Serializable]
    public class CrateDefinition
    {
        public string crateType;
        public bool isFromBundle;
        public string bundleName;
    }

    public class BoxSpawnManager : MonoBehaviour
    {
        [Header("Spawn Settings")]
        public float spawnIntervalSeconds = 1.5f;
        public Vector3 spawnOffset = new Vector3(0f, 1f, 0f);

        [Header("Crate Types")]
        public List<CrateDefinition> crateTypes = new List<CrateDefinition>
        {
            new CrateDefinition { crateType = "Wood" },
            new CrateDefinition { crateType = "Metal" },
            new CrateDefinition { crateType = "Carbon" },
            new CrateDefinition { crateType = "Heavy", isFromBundle = true, bundleName = "heavycrate" }
        };

        private float timer;

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= spawnIntervalSeconds)
            {
                timer = 0f;
                SpawnRandomCrate();
            }
        }

        private void SpawnRandomCrate()
        {
            int roll = Random.Range(0, crateTypes.Count);
            CrateDefinition chosen = crateTypes[roll];
            Vector3 spawnPos = transform.position + spawnOffset;

            if (chosen.isFromBundle)
            {
                StartCoroutine(SpawnManager.SpawnCrateAsync(chosen.bundleName, chosen.crateType, spawnPos));
            }
            else
            {
                GameObject crate = CratePoolManager.Instance.GetCrate(chosen.crateType, spawnPos);
                if (crate == null) return;

                BoxData d = crate.GetComponent<BoxData>();
                d?.SetType(chosen.crateType);

                BoxTextureLoader loader = crate.GetComponent<BoxTextureLoader>();
                loader?.LoadTextureFromStreamingAssets();

                crate.name = "Crate_" + chosen.crateType;
            }
        }
    }
}
