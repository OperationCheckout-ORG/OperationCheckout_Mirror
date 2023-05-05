using UnityEngine;

public class ParticleOnCollision : MonoBehaviour
{
    public GameObject particleEffect; // The particle effect to play on collision

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0]; // Get the first contact point of the collision
        Vector3 position = contact.point; // Get the position of the contact point

        GameObject particle = Instantiate(particleEffect, position, Quaternion.identity); // Instantiate the particle effect at the contact point position
        Destroy(particle, particle.GetComponent<ParticleSystem>().main.duration); // Destroy the particle effect after it finishes playing
    }
}