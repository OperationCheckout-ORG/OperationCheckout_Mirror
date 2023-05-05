using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int Score { get; private set; }
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] private TextAnimationHandler textAnimation;

    public static event Action<int> OnNewScore;
    
    void Awake()
    {
        Instance = this;
        AddScore(0);
    }

    public void AddScore(int scoreToAdd)
    {
        Score += scoreToAdd;
        scoreText.text = $"${Score}";
        textAnimation.ScaleAndShakeText();
        
        
        
        OnNewScore?.Invoke(Score);
    }
    
}
