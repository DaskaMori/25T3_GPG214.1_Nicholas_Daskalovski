using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Conveyor;
using Analytics;

namespace DLC
{
    public class EnemyEventManager : MonoBehaviour
    {
        [Header("DLC")]
        public EnemyBundleCatalog bundleCatalog;

        [Header("Config")]
        public List<EnemyEventDef> enemyDefs;
        public Vector2 spawnIntervalRange = new Vector2(8f, 14f);
        public int maxConcurrent = 3;
        public bool dlcOnly = true;

        [Header("UI")]
        public EnemyEventUI uiPrefab;
        public RectTransform uiParent;

        [Header("FX")]
        public ParticleSystem fxPrefab;
        public Color fxPowered    = Color.white;
        public Color fxPaused     = new Color(1f, .9f, .2f);
        public Color fxJammed     = new Color(1f, .3f, .1f);
        public Color fxReversed   = new Color(.2f, .6f, 1f);
        public Color fxOverloaded = new Color(1f, .5f, 0f);

        [Header("Startup")]
        public bool waitForConveyors = true;
        public float waitTimeout = 5f;
        
        [Header("DLC Toggle")]
        public bool dlcEnabled = true;
        public GameObject enemyMenuRoot;

        private readonly List<EnemyEventUI> activeUi = new();
        private ConveyorController[] conveyors = System.Array.Empty<ConveyorController>();
        private int concurrency;
        
        private int hazardsAvertedLifetime = 0;
        public int GetHazardsAvertedLifetime() => hazardsAvertedLifetime;

        
        void OnEnable()
        {
            ConveyorRegistry.OnListChanged += HandleConveyorListChanged;
            HandleConveyorListChanged(); 
            StartCoroutine(Bootstrap());
        }

        void OnDisable()
        {
            ConveyorRegistry.OnListChanged -= HandleConveyorListChanged;
            StopAllCoroutines();
        }

        private void HandleConveyorListChanged()
        {
            conveyors = ConveyorRegistry.All.ToArray();
            //Debug.Log($"[EnemyEventManager] Registry now has {conveyors.Length} conveyors.");
        }

        private IEnumerator Bootstrap()
        {
            if (dlcOnly) enemyDefs = new List<EnemyEventDef>();

            if (bundleCatalog)
            {
                var loaded = new List<EnemyEventDef>();
                yield return StartCoroutine(EnemyBundleLoader.LoadEnemyEvents(bundleCatalog, loaded));
                if (loaded.Count > 0)
                {
                    if (enemyDefs == null) enemyDefs = new List<EnemyEventDef>();
                    enemyDefs.AddRange(loaded);
                }
            }

            if (dlcOnly && (enemyDefs == null || enemyDefs.Count == 0))
            {
                Debug.LogError("[EnemyEventManager] DLC-only mode: no EnemyEventDef loaded from bundles.");
                yield break;
            }
            
            if (waitForConveyors)
            {
                float t = 0f;
                while (conveyors.Length == 0 && t < waitTimeout)
                {
                    t += Time.deltaTime;
                    yield return null;
                }
                if (conveyors.Length == 0)
                    Debug.LogWarning("[EnemyEventManager] No conveyors detected before timeout; events will wait until some register.");
            }

            StartCoroutine(CoEventLoop());
        }

        private IEnumerator CoEventLoop()
        {
            while (true)
            {
                if (!dlcEnabled) { yield return null; continue; }

                if (concurrency < maxConcurrent && enemyDefs != null && enemyDefs.Count > 0 && conveyors.Length > 0)
                {
                    var def = PickWeighted(enemyDefs);
                    StartCoroutine(CoRunEvent(def));
                }
                yield return new WaitForSeconds(Random.Range(spawnIntervalRange.x, spawnIntervalRange.y));
            }
        }

        private IEnumerator CoRunEvent(EnemyEventDef def)
        {
            if (!dlcEnabled) yield break;

            concurrency++;

            var targets = SelectTargets(def);
            Debug.Log($"[EnemyEvent] {def.id} targeting {targets.Count} conveyor(s) for {def.duration:0.0}s → state {def.targetState}");
            Debug.Log($"[EnemyEvent] {def.id} targeting {targets.Count} ...");
            AnalyticsManager.Instance?.LogDlc(def.id, "spawn");
            if (targets.Count == 0) { concurrency--; yield break; }
            
            var parent = uiParent ? uiParent : FindObjectOfType<Canvas>().transform as RectTransform;
            var ui = Instantiate(uiPrefab, parent);
            ui.Setup(def.icon, def.id);
            ui.BindActions(
                onA: () => ResolveRepair(ui, targets, def.id),
                onB: () => ResolveOverride(ui, targets, def.id),
                onC: () => ResolvePurge(ui, targets, def.id)
            );
            activeUi.Add(ui);

            Color tint = FxColorFor(def.targetState);
            foreach (var c in targets)
            {
                c.SetStateForDuration(def.targetState, def.duration);

                if (fxPrefab != null)
                {
                    var anchor = (c as object as MonoBehaviour)?.transform; 
                    Vector3 pos = c.transform.position + Vector3.up * 0.35f;
                    var fx = Instantiate(fxPrefab, anchor ? pos : c.transform.position + Vector3.up * 0.35f,
                                         Quaternion.identity, c.transform);
                    var main = fx.main; main.startColor = tint;
                    fx.Play();
                    Destroy(fx.gameObject, def.duration + 0.25f);
                }
            }

            float t = def.duration;
            while (t > 0f)
            {
                ui.SetTime(t);
                t -= Time.deltaTime;
                yield return null;
            }

            if (activeUi.Remove(ui)) Destroy(ui.gameObject);
            concurrency--;
        }

        private List<ConveyorController> SelectTargets(EnemyEventDef def)
        {
            var list = new List<ConveyorController>();
            if (conveyors == null || conveyors.Length == 0) return list;

            if (def.affectAllConveyors) { list.AddRange(conveyors); return list; }

            var pool = new List<ConveyorController>(conveyors);
            int n = Mathf.Clamp(def.affectCount, 1, pool.Count);
            for (int i = 0; i < n; i++)
            {
                int idx = Random.Range(0, pool.Count);
                list.Add(pool[idx]);
                pool.RemoveAt(idx);
            }
            return list;
        }

        private void ResolveRepair(EnemyEventUI ui, List<ConveyorController> targets, string eventId)
        {
            foreach (var c in targets) c.SetState(ConveyorStateId.Powered);
            hazardsAvertedLifetime++;
            AnalyticsManager.Instance?.LogDlc(eventId, "resolve_repair");
            CloseUI(ui);
        }

        private void ResolveOverride(EnemyEventUI ui, List<ConveyorController> targets, string eventId)
        {
            foreach (var c in targets) c.SetStateForDuration(ConveyorStateId.Powered, 5f);
            hazardsAvertedLifetime++;
            AnalyticsManager.Instance?.LogDlc(eventId, "resolve_override");
            CloseUI(ui);
        }

        private void ResolvePurge(EnemyEventUI ui, List<ConveyorController> targets, string eventId)
        {
            foreach (var c in targets) c.SetStateForDuration(ConveyorStateId.Jammed, 2f);
            hazardsAvertedLifetime++;
            AnalyticsManager.Instance?.LogDlc(eventId, "resolve_purge");
            CloseUI(ui);
        }

        private void CloseUI(EnemyEventUI ui)
        {
            if (activeUi.Remove(ui)) Destroy(ui.gameObject);
        }
        
        public void SetDlcEnabled(bool on)
        {
            dlcEnabled = on;

            if (enemyMenuRoot != null)
                enemyMenuRoot.SetActive(on);

            if (!on)
            {
                StopAllCoroutines();
                concurrency = 0;

                foreach (var ui in activeUi)
                    if (ui != null) Destroy(ui.gameObject);

                activeUi.Clear();
            }
            else
            {
                StartCoroutine(Bootstrap());
            }
        }
        
        public void ToggleDlc() => SetDlcEnabled(!dlcEnabled);


        private static EnemyEventDef PickWeighted(List<EnemyEventDef> defs)
        {
            float total = 0f; foreach (var d in defs) total += Mathf.Max(0.0001f, d.weight);
            float r = Random.value * total;
            foreach (var d in defs) { r -= Mathf.Max(0.0001f, d.weight); if (r <= 0f) return d; }
            return defs[defs.Count - 1];
        }

        private Color FxColorFor(ConveyorStateId s) =>
            s switch
            {
                ConveyorStateId.Powered => fxPowered,
                ConveyorStateId.Paused => fxPaused,
                ConveyorStateId.Jammed => fxJammed,
                ConveyorStateId.Reversed => fxReversed,
                ConveyorStateId.Overloaded => fxOverloaded,
                _ => Color.white
            };
        
        public void ApplyLoadedHazardsAverted(int total)
        {
            hazardsAvertedLifetime = Mathf.Max(0, total);
            Debug.Log($"[EnemyEventManager] Loaded hazards averted: {hazardsAvertedLifetime}");
        }
    }
}
