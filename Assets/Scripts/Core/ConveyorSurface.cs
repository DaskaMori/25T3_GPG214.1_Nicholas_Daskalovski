using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ConveyorSurface : MonoBehaviour
{
    public Vector3 conveyorDirection = new Vector3(1f, 0f, 0f);
    public float speed = 2f;
    public float stickForce = 20f;

    private void OnCollisionStay(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb == null)
        {
            return;
        }

        Vector3 dir = conveyorDirection.normalized;
        Vector3 targetVelocity = dir * speed;
        
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 newVelocity = new Vector3(targetVelocity.x, currentVelocity.y, targetVelocity.z);
        rb.linearVelocity = newVelocity;
        
        rb.AddForce(Vector3.down * stickForce, ForceMode.Acceleration);
    }
}

