using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Audio Library")]
public class AudioLibrary : ScriptableObject
{
    [Header("UI")]
    public AudioCue button;
    public AudioCue lockedButton;

    [Header("Drone")]
    public AudioCue spray;
    public AudioCue droneFly;
    public AudioCue droneLand;
    public AudioCue droneLift;

    [Header("Plants")]
    public AudioCue plantDone;

    [Header("Ambience")]
    public AudioCue lostSignal;
    public AudioCue wind;
    public AudioCue birds;
    public AudioCue gameplayAmbience;

    [Header("Music")]
    public AudioCue menuMusic;
    public AudioCue gameplayMusic;
    public AudioCue transitionMusic;

    [Header("Gameplay")]
    public AudioCue timerWarning;
    public AudioCue timeout;
    public AudioCue resultScreen;

    [Header("Transition")]
    public AudioCue countdown;
    public AudioCue countdownFinished;
}