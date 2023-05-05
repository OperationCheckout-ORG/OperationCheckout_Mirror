using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int _maxTime;
    [SerializeField] private float _timeRemaining;
    [SerializeField] private TextMeshProUGUI _timeText;

    [SerializeField] private PauseMenuController _pauseMenu;
    [SerializeField] private GameOverController _gameOverMenu;
    [SerializeField] private CartController _player;

    [SerializeField] private AudioManager _audioManager;
    private Coroutine _alertSoundCoroutine;
    private readonly WaitForSeconds _waitForSecondsForAlertSound = new WaitForSeconds(1f);

    public enum GameStates
    {
        Paused,
        Playing,
        GameOver
    }

    public GameStates GameState { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    private void Start()
    {
        _timeRemaining = _maxTime;
        _player = FindObjectOfType<CartController>();
        GameState = GameStates.Playing;
        CheckStates();
    }

    private void Update()
    {
        if (_timeRemaining > 0f && GameState == GameStates.Playing)
        {
            _timeRemaining = Mathf.Clamp(_timeRemaining - Time.deltaTime, 0f, _timeRemaining);
            UpdateTimeText();
            UpdateTimerSound();
        }
        else if (_timeRemaining <= 0f && GameState != GameStates.GameOver)
        {
            GameState = GameStates.GameOver;
            
            CheckStates();
        }
    }


    private void UpdateTimeText()
    {
        string mins = TimeSpan.FromSeconds(_timeRemaining).Minutes.ToString("00");
        string secs = TimeSpan.FromSeconds(_timeRemaining).Seconds.ToString("00");

        if (_timeRemaining <= 10f && _timeText.color != Color.red)
        {
            _timeText.color = Color.red;
        }

        _timeText.text = $"{mins}:{secs}";
    }

    private void UpdateTimerSound()
    {
        if (_timeRemaining <= 10f && _alertSoundCoroutine == null)
        {
            _alertSoundCoroutine = StartCoroutine(PlayTimerSound());
        }
        else if (_timeRemaining > 10f && _alertSoundCoroutine != null)
        {
            StopCoroutine(_alertSoundCoroutine);
            _alertSoundCoroutine = null;
        }
        else if (_timeRemaining <= 0f && _alertSoundCoroutine != null)
        {
            StopCoroutine(_alertSoundCoroutine);
            _alertSoundCoroutine = null;
        }
    }

    private IEnumerator PlayTimerSound()
    {
        while (_timeRemaining <= 10f)
        {
            _audioManager.PlaySFX(4);
            yield return _waitForSecondsForAlertSound;
        }
    }

    public void CheckStates()
    {
        switch (GameState)
        {
            case GameStates.Playing:
                _player.ToggleSkids(false);
                _pauseMenu.TogglePause(false);
                break;
            case GameStates.Paused:
                _player.ToggleSkids(true);
                _pauseMenu.TogglePause(true);
                break;
            case GameStates.GameOver:
                _player.ToggleSkids(true);
                _gameOverMenu.GameOver();
                break;
        }
    }
}
