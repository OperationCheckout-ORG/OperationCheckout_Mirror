using UnityEngine;

public class DashStretch : MonoBehaviour
{
    [SerializeField] private float stretchScale = 1.5f;
    [SerializeField] private float stretchDuration = 0.1f;
    [SerializeField] private float squashScale = 0.8f;
    [SerializeField] private float squashDuration = 0.05f;

    private Vector3 initialScale;
    private float stretchTimer;
    private float squashTimer;
    private bool isStretching;
    private bool isSquashing;

    private void Start()
    {
        initialScale = transform.localScale;
    }

    private void Update()
    {
        if (isStretching)
        {
            stretchTimer += Time.deltaTime;

            // Stretch the player model along the z-axis only
            float t = Mathf.Clamp01(stretchTimer / stretchDuration);
            float scaleZ = Mathf.Lerp(initialScale.z, initialScale.z * stretchScale, t);
            transform.localScale = new Vector3(initialScale.x, initialScale.y, scaleZ);

            // End the stretch after the duration
            if (stretchTimer >= stretchDuration)
            {
                isStretching = false;
                stretchTimer = 0f;

                // Start the squash animation
                isSquashing = true;
            }
        }

        if (isSquashing)
        {
            squashTimer += Time.deltaTime;

            // Squash the player model
            float t = Mathf.Clamp01(squashTimer / squashDuration);
            float squashZ = Mathf.Lerp(initialScale.z * stretchScale, initialScale.z * squashScale, t);
            transform.localScale = new Vector3(initialScale.x, initialScale.y, squashZ);

            // End the squash after the duration
            if (squashTimer >= squashDuration)
            {
                isSquashing = false;
                squashTimer = 0f;

                // Return to the original scale
                transform.localScale = initialScale;
            }
        }
    }

    public void StartStretch()
    {
        isStretching = true;
    }
}
