using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("General")]
    public string levelName;

    [Header("Scene")]
    public string sceneName;

    [TextArea(5, 10)]
    public string description;

    public Sprite previewImage;

    [Header("Mission")]
    public string cropType;
    public string location;
    public string objective;

    public int difficulty;
}