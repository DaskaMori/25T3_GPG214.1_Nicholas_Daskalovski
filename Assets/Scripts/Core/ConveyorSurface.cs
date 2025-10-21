using UnityEngine;

namespace Core
{
    public class ConveyorSurface : MonoBehaviour
    {
        [Header("Conveyor Settings")]
        public Vector3 conveyorDirection = new Vector3(1f, 0f, 0f);
        public float baseSpeed = 2f;
        public float stickForce = 20f;

        private void OnCollisionStay(Collision collision)
        {
            Rigidbody rb = collision.rigidbody;
            if (rb == null) return;

            float centerThreshold = 0.1f; 
            Vector3 conveyorCenter = transform.position;
            Vector3 boxPosition = rb.worldCenterOfMass;

            float distance = Vector3.Distance(
                conveyorCenter,
                new Vector3(boxPosition.x, conveyorCenter.y, boxPosition.z)
            );

            if (distance > centerThreshold)
                return; 

            BoxData boxData = rb.GetComponent<BoxData>();
            float weight = 1f;
            if (boxData != null)
            {
                weight = Mathf.Max(boxData.weight, 0.1f); // avoid divide-by-zero
            }

            Vector3 dir = conveyorDirection.normalized;
            Vector3 targetVelocity = dir * (baseSpeed / weight);

            Vector3 currentVelocity = rb.linearVelocity;
            Vector3 newVelocity = new Vector3(targetVelocity.x, currentVelocity.y, targetVelocity.z);
            rb.linearVelocity = newVelocity;

            rb.AddForce(Vector3.down * stickForce, ForceMode.Acceleration);
        }

    }
}