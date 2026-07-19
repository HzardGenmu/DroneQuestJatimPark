using UnityEngine;

public class MusicController : MonoBehaviour
{
    private AudioHandle musicHandle;
    private AudioHandle ambienceHandle;

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
        AudioManager.Instance.Stop(musicHandle);
        musicHandle = null;

        AudioManager.Instance.Stop(ambienceHandle);
        ambienceHandle = null;

        switch (state)
        {
            case GameState.MainMenu:

                musicHandle =
                    AudioManager.Instance.Play(
                        AudioManager.Instance.audioLibrary.menuMusic);

                break;

            case GameState.Gameplay:

                musicHandle =
                    AudioManager.Instance.Play(
                        AudioManager.Instance.audioLibrary.gameplayMusic);

                ambienceHandle =
                    AudioManager.Instance.Play(
                        AudioManager.Instance.audioLibrary.gameplayAmbience);

                break;

            case GameState.Transition:

                musicHandle =
                    AudioManager.Instance.Play(
                        AudioManager.Instance.audioLibrary.transitionMusic);

                break;

            case GameState.Summary:

                break;
        }
    }
}