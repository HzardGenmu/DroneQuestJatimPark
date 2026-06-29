using UnityEngine;

public class LevelPage : MonoBehaviour
{
    [Header("References")]
    public RectTransform Root;

    public RectTransform Background;
    public RectTransform Midground;
    public RectTransform Foreground;
    public RectTransform UI;

    [HideInInspector] public Vector2 BackgroundStart;
    [HideInInspector] public Vector2 MidgroundStart;
    [HideInInspector] public Vector2 ForegroundStart;
    [HideInInspector] public Vector2 UIStart;

    private void Awake()
    {
        BackgroundStart = Background.anchoredPosition;
        MidgroundStart = Midground.anchoredPosition;
        ForegroundStart = Foreground.anchoredPosition;
        UIStart = UI.anchoredPosition;
    }
}