using UnityEngine;

public class MusicController : MonoBehaviour
{
    private AudioHandle musicHandle;
    private AudioHandle ambienceHandle;
    private AudioCue currentMusic;
    private AudioCue currentAmbience;

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += SwitchMusic;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= SwitchMusic;
    }

    private void Start()
    {
        SwitchMusic(GameManager.Instance.CurrentState);
    }

    private void SwitchMusic(GameState state)
    {
        AudioCue targetMusic = null;
        AudioCue targetAmbience = null;

        switch (state)
        {
            case GameState.MainMenu:
            case GameState.MapSelect:
                targetMusic =
                    AudioManager.Instance.audioLibrary.menuMusic;
                break;

            case GameState.Gameplay:
                targetMusic =
                    AudioManager.Instance.audioLibrary.gameplayMusic;

                targetAmbience =
                    AudioManager.Instance.audioLibrary.gameplayAmbience;
                break;

            case GameState.Transition:
                targetMusic =
                    AudioManager.Instance.audioLibrary.transitionMusic;
                break;
        }

        //
        // Music
        //

        if (targetMusic != currentMusic)
        {
            AudioManager.Instance.Stop(musicHandle);

            musicHandle = null;

            currentMusic = targetMusic;

            if (currentMusic != null)
            {
                musicHandle =
                    AudioManager.Instance.Play(currentMusic);
            }
        }

        //
        // Ambience
        //

        if (targetAmbience != currentAmbience)
        {
            AudioManager.Instance.Stop(ambienceHandle);

            ambienceHandle = null;

            currentAmbience = targetAmbience;

            if (currentAmbience != null)
            {
                ambienceHandle =
                    AudioManager.Instance.Play(currentAmbience);
            }
        }
    }
}