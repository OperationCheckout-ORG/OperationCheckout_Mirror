using UnityEngine;

public class Shelf : MonoBehaviour {

    public float collisionForceThreshold = 5f;
    public float dampingFactor = 0.1f;

    private Rigidbody[] rigidbodies;

    void Start() {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        DisableRigidbodies();
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.relativeVelocity.magnitude > collisionForceThreshold) {
            EnableRigidbodies();
            Vector3 centerOfCollision = Vector3.zero;
            foreach (ContactPoint contact in collision.contacts) {
                centerOfCollision += contact.point;
            }
            centerOfCollision /= collision.contacts.Length;
            foreach (Rigidbody rb in rigidbodies) {
                Vector3 forceDirection = rb.position - centerOfCollision;
                float distanceFromCenter = forceDirection.magnitude;
                forceDirection.Normalize();
                Vector3 force = collision.relativeVelocity * collision.rigidbody.mass * dampingFactor * (1f / rb.mass);
                Vector3 dampingForce = -rb.velocity * dampingFactor * rb.mass;
                Vector3 normalForceDirection = -collision.contacts[0].normal;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.velocity = force * Mathf.Clamp01(1f - distanceFromCenter / 2f);
                rb.angularVelocity = normalForceDirection * collision.relativeVelocity.magnitude * collision.rigidbody.mass * 0.5f * (1f / rb.mass);
                rb.velocity += dampingForce;
            }
        }
    }

    void DisableRigidbodies() {
        foreach (Rigidbody rb in rigidbodies) {
            rb.isKinematic = true;
        }
    }

    void EnableRigidbodies() {
        foreach (Rigidbody rb in rigidbodies) {
            rb.isKinematic = false;
        }
    }
}