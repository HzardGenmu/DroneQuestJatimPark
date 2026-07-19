using UnityEngine;

public enum AudioChannel
{
    UI,
    SFX,
    Ambience,
    Music,
    Drone
}

[System.Serializable]
public class AudioCue
{
    [Header("Audio")]
    public AudioClip clip;

    [Header("Channel")]
    public AudioChannel channel;

    [Header("Playback")]
    public bool loop;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(0.5f, 2f)]
    public float pitch = 1f;

    [Header("Trim")]

    [Min(0)]
    public float startTime;

    [Min(0)]
    public float endTime;

    [Header("Random Pitch")]

    public bool randomPitch;

    [Range(0f, 0.3f)]
    public float pitchVariation = 0.05f;
}