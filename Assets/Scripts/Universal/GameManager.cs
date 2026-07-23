using UnityEngine;
using UnityEngine.SceneManagement;


public enum GameState
{
    MainMenu,
    MapSelect,
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
        CurrentLevel = level;

        AudioManager.Instance.StopChannel(AudioChannel.SFX);
        AudioManager.Instance.StopChannel(AudioChannel.Ambience);

        // Stop gameplay-only audio BEFORE notifying listeners.
        if (state == GameState.MainMenu ||
            state == GameState.MapSelect)
        {
            AudioManager.Instance.StopGameplayAudio();
        }

        if (CurrentState != state)
        {
            CurrentState = state;

            Debug.Log($"Game State -> {state}");

            OnGameStateChanged?.Invoke(state);
        }

        switch (state)
        {
            case GameState.Gameplay:

                if (level != null)
                    SceneManager.LoadScene(level.sceneName);

                break;

            case GameState.MainMenu:

                AudioManager.Instance.StopGameplayAudio();
                SceneManager.LoadScene("MainMenu");

                break;

            case GameState.MapSelect:

                AudioManager.Instance.StopGameplayAudio();
                SceneManager.LoadScene("MapSelector");

                break;

            case GameState.Summary:

                SceneManager.LoadScene("Summary");

                break;

            case GameState.Transition:

                SceneManager.LoadScene("Transition");

                break;
        }

        CurrentState = state;
        OnGameStateChanged?.Invoke(state);
    }
}