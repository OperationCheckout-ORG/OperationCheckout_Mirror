using System.Collections;
using UnityEngine;

using System.Collections;
using UnityEngine;

public class CartStretch : MonoBehaviour
{
    [Tooltip("The transform of the player model that will stretch.")]
    public Transform playerModel;

    [Tooltip("The amount that the player model will stretch, relative to the turn speed.")]
    public float stretchAmount = 0.2f;

    [Tooltip("The maximum amount that the player model can stretch.")]
    public float maxStretch = 1.5f;

    [Tooltip("The speed at which the player model stretches.")]
    public float stretchSpeed = 10f;

    [Tooltip("The number of times the stretch effect will repeat during the falloff.")]
    public int numFalloffs = 5;

    [Tooltip("The amount by which the stretch effect will decrease during the falloff.")]
    public float falloffAmount = 0.5f;

    [Tooltip("The speed at which the stretch effect will decrease during the falloff.")]
    public float falloffSpeed = 5f;

    [Tooltip("The minimum speed required for the stretch effect to be applied.")]
    public float minSpeedForStretch = 5f;

    private float targetStretch;
    private Vector3 defaultScale;
    private float currentStretch;
    private bool isFalloff;
    private float falloffScale;

    void Start()
    {
        defaultScale = playerModel.localScale;
        currentStretch = 0;
        isFalloff = false;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float speed = Mathf.Abs(horizontal);

        if (speed >= minSpeedForStretch) {
            if (isFalloff) {
                currentStretch -= Time.deltaTime * falloffSpeed;
                if (currentStretch <= 0) {
                    currentStretch = 0;
                    isFalloff = false;
                }
            }
            else {
                targetStretch = Mathf.Clamp(speed * stretchAmount, 0, maxStretch);
                currentStretch = Mathf.Lerp(currentStretch, targetStretch, Time.deltaTime * stretchSpeed);

                if (Mathf.Approximately(currentStretch, targetStretch)) {
                    isFalloff = true;
                    falloffScale = currentStretch;

                    for (int i = 1; i <= numFalloffs; i++) {
                        Invoke("DoFalloff", i * 0.1f);
                    }
                }
            }
        }
        else {
            currentStretch = 0;
            isFalloff = false;
        }

        playerModel.localScale = defaultScale + Vector3.right * currentStretch;
    }

    void DoFalloff()
    {
        currentStretch = falloffScale * falloffAmount;
        falloffScale = currentStretch;

        if (Mathf.Approximately(currentStretch, 0)) {
            isFalloff = false;
        }
    }
}
