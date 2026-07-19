using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private bool paused;

    private void Start()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void TogglePause()
    {
        if (paused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        paused = true;

        pausePanel.SetActive(true);

        AudioManager.Instance.PauseAll();

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        paused = false;

        Time.timeScale = 1f;

        AudioManager.Instance.ResumeAll();

        AudioManager.Instance.Play(
            AudioManager.Instance.audioLibrary.button);
        Debug.Log(AudioManager.Instance.GetEntityId());
        pausePanel.SetActive(false);
        AudioManager.Instance.SetUIVolume(1f);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        GameManager.Instance.ChangeState(GameState.MainMenu);
    }
}