using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TutorialTriggerType
{
    OnStart,
    OnPlantScanned,
    OnCameraModeChanged,
    OnPlantTreated,
    Manual
}

public enum TutorialPanelAnchor
{
    Top,
    Bottom,
    Left,
    Right,
    Center
}

[CreateAssetMenu(menuName = "Tutorial/Step")]
public class TutorialStep : ScriptableObject
{
    [Header("Text")]
    [TextArea]
    public string description;

    [Header("Focus")]
    public List<TutorialFocusData> focusItems;

    [Header("Trigger")]
    public TutorialTriggerType triggerType;

    [Header("Game State")]
    public bool pauseGame;

    [Header("Panel Layout")]
    public TutorialPanelAnchor panelAnchor;

    public Vector2 panelOffset;

    [Header("Objects")]
    public List<GameObject> activateWhileActive = new();

    [Header("Input Prompt")]
    public bool showInputPrompt = true;

    public string inputActionName = "Submit";

    public bool autoFlipPanel = true;

    [Header("Panel Flip")]
    public bool flipPanel;

    [Header("Optional UI")]
    public Sprite image;

    [Header("Buttons")]
    public bool showButtons = true;

    public bool showPreviousButton = true;
    public bool showNextButton = true;

    public string previousButtonText = "Prev";
    public string nextButtonText = "Next";

    public string skipButtonText = "Skip";
}