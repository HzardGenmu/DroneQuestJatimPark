using UnityEngine;

public class MapSaveSystem : MonoBehaviour
{
    public static MapSaveSystem Instance;

    private const string UnlockedKey = "UnlockedLevel";
    [SerializeField] private int totalLevels = 8;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // First launch
            if (!PlayerPrefs.HasKey(UnlockedKey))
            {
                PlayerPrefs.SetInt(UnlockedKey, 1);
                PlayerPrefs.Save();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetUnlockedLevel()
    {
        return PlayerPrefs.GetInt(UnlockedKey, 1);
    }

    public bool IsUnlocked(int levelIndex)
    {
        return levelIndex <= GetUnlockedLevel();
    }

    public void UnlockNextLevel(int currentLevel)
    {
        int unlocked = GetUnlockedLevel();

        if (currentLevel >= unlocked)
        {
            int next =
                Mathf.Min(
                    currentLevel + 1,
                    totalLevels);

            PlayerPrefs.SetInt(
                UnlockedKey,
                next);

            PlayerPrefs.Save();
        }
    }

    public void SaveStars(int levelIndex, int stars)
    {
        string key = $"Stars_{levelIndex}";

        int previous =
            PlayerPrefs.GetInt(key, 0);

        // Never overwrite with a worse score
        if (stars > previous)
        {
            PlayerPrefs.SetInt(key, stars);
            PlayerPrefs.Save();
        }
    }

    public int GetStars(int levelIndex)
    {
        return PlayerPrefs.GetInt(
            $"Stars_{levelIndex}",
            0);
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();

        PlayerPrefs.SetInt(UnlockedKey, 1);

        PlayerPrefs.Save();
    }
}