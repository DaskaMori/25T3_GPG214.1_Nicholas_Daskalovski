using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Core.Conveyor
{
    [DefaultExecutionOrder(-10)]
    public class ConveyorController : MonoBehaviour
    {
        [Header("Conveyor Settings")]
        public Vector3 conveyorDirection = new Vector3(1f, 0f, 0f);
        public float baseSpeed = 2f;
        public float stickForce = 20f;

        [Header("State")]
        [SerializeField] private ConveyorStateId initialState = ConveyorStateId.Powered;
        [SerializeField] private ConveyorStateId currentStateId;

        private readonly HashSet<Rigidbody> crates = new HashSet<Rigidbody>();
        private readonly Dictionary<ConveyorStateId, IConveyorState> states = new();
        private IConveyorState currentState;
        private float lastTransitionTime;
        private const float TransitionDebounce = 0.05f;

        public IReadOnlyCollection<Rigidbody> Crates => crates;
        public Vector3 DirNorm => conveyorDirection.sqrMagnitude > 0 ? conveyorDirection.normalized : Vector3.right;

        private Coroutine timedStateCo;
        
        public Transform fxAnchor;
        
        private void OnEnable()  => ConveyorRegistry.Register(this);
        private void OnDisable() => ConveyorRegistry.Unregister(this);

        private void Awake()
        {
            states[ConveyorStateId.Powered] = new PoweredState();
            states[ConveyorStateId.Paused] = new PausedState();
            states[ConveyorStateId.Reversed] = new ReversedState();
            states[ConveyorStateId.Jammed] = new JammedState();
            states[ConveyorStateId.Overloaded] = new OverloadedState();

            SetState(initialState, force:true);
        }

        private void FixedUpdate()
        {
            currentState?.Update(this, Time.fixedDeltaTime);

            foreach (var rb in crates)
            {
                if (!rb) continue;
                rb.AddForce(Vector3.down * stickForce, ForceMode.Acceleration);
            }
            
            //Debug.Log($"STATE={currentStateId} | crates={crates.Count}");
        }

        public void RegisterCrate(Rigidbody rb)   { if (rb) crates.Add(rb); }
        public void UnregisterCrate(Rigidbody rb) { if (rb) crates.Remove(rb); }

        public void SetState(ConveyorStateId target, bool force=false)
        {
            if (!force && currentState != null && (currentState.Id == target ||
                Time.time - lastTransitionTime < TransitionDebounce || !currentState.Allows(target))) return;

            if (!states.TryGetValue(target, out var next)) return;
            var prev = currentStateId;
            currentState?.OnExit(this);
            currentState = next;
            currentStateId = next.Id;
            currentState.OnEnter(this);
            lastTransitionTime = Time.time;

            // add hook for  analytics 
            // Debug.Log($"Conveyor: {prev} -> {currentStateId}");
        }

        public ConveyorStateId GetState() => currentStateId;

        public static float GetWeight(Rigidbody rb)
        {
            var data = rb.GetComponent<Core.BoxData>();
            return data ? Mathf.Max(data.weight, 0.1f) : 1f;
        }
        public static void SetXZVelocity(Rigidbody rb, Vector3 xz)
        {
            var v = rb.linearVelocity;
            rb.linearVelocity = new Vector3(xz.x, v.y, xz.z);
        }
        
        public void SetStateForDuration(ConveyorStateId state, float seconds)
        {
            if (timedStateCo != null) StopCoroutine(timedStateCo);
            timedStateCo = StartCoroutine(CoTimedState(state, seconds));
        }

        private IEnumerator CoTimedState(ConveyorStateId state, float seconds)
        {
            var prev = GetState();               
            SetState(state);                     
            yield return new WaitForSeconds(seconds);
            SetState(prev);                      
            timedStateCo = null;
        }
    }
}
