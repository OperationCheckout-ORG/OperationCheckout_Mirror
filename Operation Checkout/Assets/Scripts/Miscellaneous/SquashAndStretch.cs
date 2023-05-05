using System.Collections;
using UnityEngine;

public class SquashAndStretch : MonoBehaviour
{
    public float squashFactor = 0.2f; // How much to squash the object
    public float stretchFactor = 0.2f; // How much to stretch the object
    public float duration = 0.2f; // How long the squash and stretch animation should take
    public float backCollisionThreshold = -0.5f; // Threshold value for detecting back collisions
    public int repeatCount = 3; // Number of times to repeat the animation
    public float repeatScaleFactor = 0.8f; // Scale factor for each repeat
    public float repeatDurationFactor = 0.8f; // Duration factor for each repeat

    private Vector3 originalScale; // The object's original scale
    private bool isSquashingOrStretching = false; // Flag to prevent multiple simultaneous animations

    void Start()
    {
        originalScale = transform.localScale; // Save the object's original scale
    }

    void OnCollisionEnter(Collision collision)
    {
        Vector3 contactNormal = collision.contacts[0].normal; // Get the contact normal of the collision
        Vector3 objectForward = transform.forward; // Get the forward direction of the object
        Vector3 objectBackward = -objectForward; // Get the backward direction of the object

        float frontDotProduct = Vector3.Dot(contactNormal, objectForward);
        float backDotProduct = Vector3.Dot(contactNormal, objectBackward);

        if (!isSquashingOrStretching && (frontDotProduct < 0f || backDotProduct < backCollisionThreshold)) // Check if collision occurred from the front or back of the object
        {
            isSquashingOrStretching = true;
            StartCoroutine(SquashAndStretchAnimation());
        }
    }

    IEnumerator SquashAndStretchAnimation()
    {
        float currentDuration = duration;
        float currentScaleFactor = 1f;

        for (int i = 0; i < repeatCount; i++)
        {
            // Squash the object
            Vector3 squashScale = originalScale - new Vector3(originalScale.x * squashFactor * currentScaleFactor, originalScale.y * squashFactor * currentScaleFactor, originalScale.z * squashFactor * currentScaleFactor);
            yield return SquashOrStretch(squashScale, currentDuration / 2f);

            // Stretch the object
            Vector3 stretchScale = originalScale + new Vector3(originalScale.x * stretchFactor * currentScaleFactor, originalScale.y * stretchFactor * currentScaleFactor, originalScale.z * stretchFactor * currentScaleFactor);
            yield return SquashOrStretch(stretchScale, currentDuration / 2f);

            // Update scale and duration for next repeat
            currentScaleFactor *= repeatScaleFactor;
            currentDuration *= repeatDurationFactor;
        }

        // Reset the object's scale
        transform.localScale = originalScale;

        isSquashingOrStretching = false;
    }

    IEnumerator SquashOrStretch(Vector3 targetScale, float animationDuration)
    {
        float elapsedTime = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsedTime < animationDuration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, (elapsedTime / animationDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }
}
