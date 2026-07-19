using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public LevelData CurrentLevel { get; private set; }

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
    }

    public void SetCurrentLevel(LevelData level)
    {
        CurrentLevel = level;
    }

    public void ClearCurrentLevel()
    {
        CurrentLevel = null;
    }

    public void StartLevel(LevelData level)
    {
        CurrentLevel = level;

        SceneManager.LoadScene(level.sceneName);
    }
}