using UnityEngine;

public class ScoreUpdate : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ScoreManager.Instance.AddScore(1);
        }
    }
}