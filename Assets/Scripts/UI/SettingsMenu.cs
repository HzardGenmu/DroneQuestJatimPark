using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    [Header("Sliders")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        panel.SetActive(false);

        bgmSlider.value =
            PlayerPrefs.GetFloat(
                "MusicVolume",
                1f);

        sfxSlider.value =
            PlayerPrefs.GetFloat(
                "SFXVolume",
                1f);

        bgmSlider.onValueChanged.AddListener(SetMusic);

        sfxSlider.onValueChanged.AddListener(SetSFX);
    }

    private void SetMusic(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    private void SetSFX(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }

    public void Open()
    {
        panel.SetActive(true);
    }

    public void Close()
    {
        panel.SetActive(false);
    }
}