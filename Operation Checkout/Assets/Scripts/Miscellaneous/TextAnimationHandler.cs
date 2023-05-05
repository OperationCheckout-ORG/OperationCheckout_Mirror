using UnityEngine;
using TMPro;
using DG.Tweening;

public class TextAnimationHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private float animationDuration = 0.2f;
    [SerializeField] private float scaleAmount = 1.1f;
    [SerializeField] private float shakeAmount = 10f;

    public void ScaleAndShakeText()
    {
        // Scale animation
        textMeshProUGUI.transform.DOPunchScale(new Vector3(scaleAmount, scaleAmount, 0), animationDuration, 1, 0.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                // Shake animation
                textMeshProUGUI.transform.DOShakePosition(animationDuration, new Vector3(shakeAmount, shakeAmount, 0), 10, 90, false, true)
                    .SetEase(Ease.InOutQuad);
            });
    }
}