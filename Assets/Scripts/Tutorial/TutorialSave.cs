using UnityEngine;

public static class TutorialSave
{
    private const string COMPLETED_KEY = "TUTORIAL_COMPLETED";
    private const string STARTED_KEY = "TUTORIAL_STARTED";

    public static bool IsCompleted()
    {
        return PlayerPrefs.GetInt(COMPLETED_KEY, 0) == 1;
    }

    public static bool HasStarted()
    {
        return PlayerPrefs.GetInt(STARTED_KEY, 0) == 1;
    }

    public static void SetStarted()
    {
        PlayerPrefs.SetInt(STARTED_KEY, 1);
        PlayerPrefs.Save();
    }

    public static void SetCompleted()
    {
        PlayerPrefs.SetInt(COMPLETED_KEY, 1);
        PlayerPrefs.Save();
    }

}