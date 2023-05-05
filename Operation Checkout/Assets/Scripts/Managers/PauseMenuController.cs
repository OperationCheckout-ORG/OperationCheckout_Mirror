using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject controlMenu;
    public void TogglePause(bool isPaused)
    {
        Time.timeScale = isPaused ? 0f : 1f;
        
        pauseMenu.SetActive(isPaused);
        GameManager.GameStates lastState = GameManager.Instance.GameState;
        if (isPaused)
        {
            GameManager.Instance.GameState = GameManager.GameStates.Paused;
        }
        else
        {
            GameManager.Instance.GameState = GameManager.GameStates.Playing;
            if(controlMenu.activeInHierarchy)
            {
                controlMenu.SetActive(false);
            }
        }
        if (lastState != GameManager.Instance.GameState)
        {
            GameManager.Instance.CheckStates();
        }
    }

    public void Restart()
    {
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
