using UnityEngine;

namespace Core.Conveyor
{
    [RequireComponent(typeof(Collider))]
    public class ConveyorSurfaceRegister : MonoBehaviour
    {
        [SerializeField] private ConveyorController controller;  // leave empty on prefab
        public float centerThreshold = 0.1f;

        private void Reset()      { TryAssign(); }
        private void OnValidate() { TryAssign(); }
        private void Awake()      { TryAssign(); }

        private void TryAssign()
        {
            if (controller == null) controller = GetComponentInParent<ConveyorController>();
        }

        private void OnCollisionEnter(Collision c)
        {
            if (!controller) return;
            var rb = c.rigidbody; if (!rb) return;

            Vector3 center = controller.transform.position;
            Vector3 p = rb.worldCenterOfMass;
            float d = Vector3.Distance(center, new Vector3(p.x, center.y, p.z));
            if (d <= centerThreshold) controller.RegisterCrate(rb);
        }

        private void OnCollisionExit(Collision c)
        {
            if (!controller) return;
            var rb = c.rigidbody; if (!rb) return;
            controller.UnregisterCrate(rb);
        }
    }
}