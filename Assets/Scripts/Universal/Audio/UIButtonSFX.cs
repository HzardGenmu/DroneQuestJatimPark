using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSFX :
    MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    ISelectHandler
{
    private static float lastPlayTime;
    private const float Cooldown = 0.05f;

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayButtonSFX();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayButtonSFX();
    }

    public void OnSelect(BaseEventData eventData)
    {
        PlayButtonSFX();
    }

    private void PlayButtonSFX()
    {
        if (AudioManager.Instance == null)
        {
            return;
        }

        Debug.Log(Time.unscaledTime - lastPlayTime);

        if (Time.unscaledTime - lastPlayTime < Cooldown)
        {
            return;
        }

        var cue = AudioManager.Instance.audioLibrary.button;

        Debug.Log(cue);
        Debug.Log(cue.clip);

        AudioManager.Instance.Play(cue);
        lastPlayTime = Time.unscaledTime;

        AudioManager.Instance.Play(AudioManager.Instance.audioLibrary.button);

    }
}