using UnityEngine;

public class AudioHandle
{
    private AudioSource source;

    public AudioSource Source => source;

    public bool IsValid =>
        source != null;

    public AudioHandle(AudioSource source)
    {
        this.source = source;
    }

    public void Stop()
    {
        if (source == null)
            return;

        source.Stop();
    }

    public void Pause()
    {
        if (source == null)
            return;

        source.Pause();
    }

    public void Resume()
    {
        if (source == null)
            return;

        source.UnPause();
    }

    public bool IsPlaying
    {
        get
        {
            if (source == null)
                return false;

            return source.isPlaying;
        }
    }
}