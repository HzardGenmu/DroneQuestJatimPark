using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject quitPanel;
    [SerializeField] private SettingsMenu settingsMenu;

    private readonly Stack<GameObject> panelStack = new();

    private bool paused;

    private void Start()
    {
        pausePanel.SetActive(false);
        quitPanel.SetActive(false);

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

        AudioManager.Instance.PauseAll();

        Time.timeScale = 0f;

        panelStack.Clear();

        ShowPanel(pausePanel);
    }

    public void ResumeGame()
    {
        paused = false;

        Time.timeScale = 1f;

        AudioManager.Instance.ResumeAll();
        AudioManager.Instance.Play(AudioManager.Instance.audioLibrary.button);

        while (panelStack.Count > 0)
        {
            panelStack.Pop().SetActive(false);
        }

        settingsMenu.Close();
    }

    private void ShowPanel(GameObject panel)
    {
        if (panelStack.Count > 0)
            panelStack.Peek().SetActive(false);

        panel.SetActive(true);
        panelStack.Push(panel);
    }

    public void Back()
    {
        if (panelStack.Count <= 1)
            return;

        panelStack.Pop().SetActive(false);

        panelStack.Peek().SetActive(true);
    }

    public void OpenSettings()
    {
        if (panelStack.Count > 0)
            panelStack.Peek().SetActive(false);

        settingsMenu.Open();
    }

    public void CloseSettings()
    {
        settingsMenu.Close();

        if (panelStack.Count > 0)
            panelStack.Peek().SetActive(true);
    }

    public void OpenQuitPanel()
    {
        ShowPanel(quitPanel);
    }

    public void CloseQuitPanel()
    {
        Back();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        AudioManager.Instance.StopGameplayAudio();

        GameManager.Instance.ChangeState(GameState.MapSelect);
    }
}