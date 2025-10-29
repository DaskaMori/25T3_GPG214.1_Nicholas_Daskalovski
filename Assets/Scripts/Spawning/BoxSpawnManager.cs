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
        
        public void SaveStateTo(GameState state)
        {
            state.activeCrates.Clear();
            foreach (var crate in GameObject.FindGameObjectsWithTag("Crate"))
            {
                var data = crate.GetComponent<BoxData>();
                if (data != null)
                {
                    state.activeCrates.Add(new CrateSnapshot
                    {
                        crateType = data.boxType,
                        position = crate.transform.position,
                        rotation = crate.transform.rotation
                    });
                }
            }

            state.spawnTimer = this.timer;
        }
        
        public void LoadStateFrom(GameState state)
        {
            foreach (var crate in GameObject.FindGameObjectsWithTag("Crate"))
            {
                Destroy(crate); 
            }

            foreach (var snapshot in state.activeCrates)
            {
                GameObject crate = CratePoolManager.Instance.GetCrate(snapshot.crateType, snapshot.position);
                if (crate == null) continue;

                crate.transform.rotation = snapshot.rotation;

                var data = crate.GetComponent<BoxData>();
                data?.SetType(snapshot.crateType);

                var loader = crate.GetComponent<BoxTextureLoader>();
                loader?.LoadTextureFromStreamingAssets();
            }

            this.timer = state.spawnTimer;
        }

    }
}
