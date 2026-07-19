using UnityEngine;
using UnityEngine.SceneManagement;


public enum GameState
{
    MainMenu,
    Gameplay,
    Summary,
    Transition
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static event System.Action<GameState> OnGameStateChanged;
    public LevelData CurrentLevel { get; private set; }
    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        CurrentState = GameState.MainMenu;

        OnGameStateChanged?.Invoke(CurrentState);
    }

    public void ChangeState(
        GameState state,
        LevelData level = null)
    {
        if (CurrentState != state)
        {
            CurrentState = state;

            Debug.Log($"Game State -> {state}");

            OnGameStateChanged?.Invoke(state);
        }

        CurrentLevel = level;
        // Stop any gameplay audio before switching scenes.
        AudioManager.Instance.StopChannel(AudioChannel.SFX);
        AudioManager.Instance.StopChannel(AudioChannel.Ambience);

        switch (state)
        {
            case GameState.Gameplay:

                if (level != null)
                    SceneManager.LoadScene(level.sceneName);
                AudioManager.Instance.SetUIVolume(1f);
                break;

            case GameState.MainMenu:

                AudioManager.Instance.StopGameplayAudio();

                SceneManager.LoadScene("MainMenu");

                break;

            case GameState.Summary:

                SceneManager.LoadScene("Summary");

                break;

            case GameState.Transition:

                SceneManager.LoadScene("Transition");

                break;
        }
    }
}