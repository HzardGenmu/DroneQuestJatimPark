using UnityEngine;

public enum ArrowDirection
{
    Up,
    Down,
    Left,
    Right,
    Auto
}

[System.Serializable]
public class TutorialFocusData
{
    public string targetID;

    [Header("Highlight")]
    public bool autoSize = true;

    public Vector2 highlightSize =
        new Vector2(150, 150);

    public Vector2 highlightOffset;

    public float padding = 20f;

    [Header("Arrow")]
    public bool showArrow = true;

    public ArrowDirection arrowDirection = ArrowDirection.Auto;

    public Vector2 arrowOffset;

    [Header("Animation")]
    public bool pulse;
}