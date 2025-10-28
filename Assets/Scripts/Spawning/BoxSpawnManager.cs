using System.Collections;
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
        public string bundleName; // Required only if isFromBundle is true
    }

    public class BoxSpawnManager : MonoBehaviour
    {
        [Header("Base Crate Prefab")]
        public GameObject baseCratePrefab;

        [Header("Spawn Settings")]
        public float spawnIntervalSeconds = 1.5f;
        public Vector3 spawnOffset = new Vector3(0f, 1f, 0f);

        [Header("Crate Types")]
        public List<CrateDefinition> crateTypes = new List<CrateDefinition>
        {
            new CrateDefinition { crateType = "Wood", isFromBundle = false },
            new CrateDefinition { crateType = "Metal", isFromBundle = false },
            new CrateDefinition { crateType = "Carbon", isFromBundle = false },
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
                // Spawn from AssetBundle
                StartCoroutine(SpawnManager.SpawnCrateAsync(chosen.bundleName, chosen.crateType, spawnPos));
            }
            else
            {
                // Spawn from local prefab
                GameObject g = Instantiate(baseCratePrefab, spawnPos, Quaternion.identity);

                BoxData d = g.GetComponent<BoxData>();
                if (d == null)
                {
                    Debug.LogError("[BoxSpawnManager] Spawned crate is missing BoxData.");
                    return;
                }

                d.SetType(chosen.crateType);

                BoxTextureLoader loader = g.GetComponent<BoxTextureLoader>();
                loader?.LoadTextureFromStreamingAssets();

                g.name = "Box_" + chosen.crateType;
            }
        }
    }
}
