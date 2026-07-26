using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Library")]
    public AudioLibrary audioLibrary;

    [Header("Dedicated Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource droneSource;

    [Header("Pooled Sources")]
    [SerializeField] private AudioSource[] sfxPool;

    [Header("Volumes")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float uiVolume = 1f;

    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";
    private const string UIVolumeKey = "UIVolume";

    private Coroutine musicFadeRoutine;

    private readonly List<AudioSource> activeLoops =
        new List<AudioSource>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        Debug.Log($"AudioManager Awake {GetEntityId()}");

        musicVolume =
            PlayerPrefs.GetFloat(
                MusicVolumeKey,
                1f);

        sfxVolume =
            PlayerPrefs.GetFloat(
                SFXVolumeKey,
                1f);

        uiVolume =
            PlayerPrefs.GetFloat(
                UIVolumeKey,
                1f);

        RefreshVolumes();
        Debug.Log($"Music {musicVolume}");
        Debug.Log($"SFX {sfxVolume}");
        Debug.Log($"UI {uiVolume}");
    }

    public AudioHandle Play(AudioClip clip)
    {
        if (clip == null)
            return null;

        AudioSource source =
            GetFreeSource();

        source.Stop();

        source.clip = clip;
        source.loop = false;
        source.pitch = 1f;
        source.volume = sfxVolume;

        source.Play();

        return new AudioHandle(source);
    }

    public AudioHandle Play(AudioCue cue)
    {
        if (cue == null || cue.clip == null)
            return null;

        AudioSource source = GetSource(cue.channel);

        source.Stop();

        source.clip = cue.clip;

        //source.pitch =
        //    cue.pitch +
        //    Random.Range(
        //        -cue.randomPitch,
        //        cue.randomPitch);

        source.time = cue.startTime;

        source.loop = cue.loop;

        source.volume =
            cue.volume *
            GetChannelVolume(cue.channel);

        source.Play();

        if (!cue.loop &&
            cue.endTime > cue.startTime)
        {
            StartCoroutine(
                StopAfter(
                    source,
                    cue.endTime - cue.startTime));
        }
        Debug.Log($"{cue.clip.name} | Channel = {cue.channel}");
        //Debug.Log(uiSource.volume);
        return new AudioHandle(source);
    }

    private AudioSource GetSource(AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Music:
                return musicSource;

            case AudioChannel.Ambience:
                return ambienceSource;

            case AudioChannel.Drone:
                return droneSource;

            case AudioChannel.UI:
            case AudioChannel.SFX:
            default:
                return GetFreeSource();
        }
    }

    private AudioSource GetFreeSource()
    {
        foreach (AudioSource source in sfxPool)
        {
            if (!source.isPlaying)
                return source;
        }

        return sfxPool[0];
    }

    private float GetChannelVolume(AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Music:
                return musicVolume;

            case AudioChannel.UI:
            case AudioChannel.SFX:
            case AudioChannel.Drone:
            case AudioChannel.Ambience:
                return sfxVolume;

            default:
                return sfxVolume;
        }
    }

    private void RefreshVolumes()
    {
        musicSource.volume = musicVolume;

        ambienceSource.volume = sfxVolume;

        droneSource.volume = sfxVolume;

        foreach (AudioSource source in sfxPool)
        {
            if (source != null)
                source.volume = sfxVolume;
        }
    }

    private IEnumerator StopAfter(
    AudioSource source,
    float delay)
    {
        yield return new WaitForSeconds(delay);

        if (source != null)
            source.Stop();
    }

    public void Stop(AudioHandle handle)
    {
        if (handle == null)
            return;

        handle.Stop();
    }

    public void Pause(AudioHandle handle)
    {
        if (handle == null)
            return;

        handle.Pause();
    }

    public void Resume(AudioHandle handle)
    {
        if (handle == null)
            return;

        handle.Resume();
    }

    public void StopChannel(AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Music:

                musicSource.Stop();
                break;

            //case AudioChannel.UI:

            //    uiSource.Stop();
            //    break;
            case AudioChannel.UI:
            case AudioChannel.SFX:

                foreach (AudioSource source in sfxPool)
                    source.Stop();

                break;
            case AudioChannel.Ambience:

                ambienceSource.Stop();
                break;
            case AudioChannel.Drone:

                droneSource.Stop();
                break;

            default:

                foreach (AudioSource source in sfxPool)
                    source.Stop();

                break;
        }
    }

    public void PauseAll()
    {
        musicSource.Pause();
        ambienceSource.Pause();
        droneSource.Pause();

        foreach (AudioSource source in sfxPool)
        {
            source.Pause();
        }
    }

    public void ResumeAll()
    {
        musicSource.UnPause();
        ambienceSource.UnPause();
        droneSource.UnPause();

        foreach (AudioSource source in sfxPool)
        {
            source.UnPause();
        }
    }

    public void FadeMusic(float target, float duration)
    {
        if (musicFadeRoutine != null)
            StopCoroutine(musicFadeRoutine);

        musicFadeRoutine =
            StartCoroutine(
                FadeMusicRoutine(
                    target,
                    duration));
    }

    private IEnumerator FadeMusicRoutine(
    float target,
    float duration)
    {
        float start =
            musicSource.volume;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            musicSource.volume =
                Mathf.Lerp(
                    start,
                    target,
                    timer / duration);

            yield return null;
        }

        musicSource.volume = target;

        musicFadeRoutine = null;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);

        PlayerPrefs.SetFloat(
            MusicVolumeKey,
            musicVolume);

        PlayerPrefs.Save();

        RefreshVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);

        PlayerPrefs.SetFloat(
            SFXVolumeKey,
            sfxVolume);

        PlayerPrefs.Save();

        RefreshVolumes();
    }

    public void StopGameplayAudio()
    {
        StopChannel(AudioChannel.SFX);
        StopChannel(AudioChannel.Ambience);
        StopChannel(AudioChannel.Drone);
    }
}
