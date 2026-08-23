using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AltitudeUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DroneController drone;
    [SerializeField] private AltitudeManager altitudeManager;

    [Header("UI")]
    [SerializeField] private Image sliderFill;
    [SerializeField] private Image handle;
    [SerializeField] private TMP_Text altitudeText;

    [Header("Correct Altitude Assets")]
    [SerializeField] private Sprite correctFillSprite;
    [SerializeField] private Sprite correctHandleSprite;

    [Header("Wrong Altitude Assets")]
    [SerializeField] private Sprite warningFillSprite;
    [SerializeField] private Sprite warningHandleSprite;

    [Header("Text Colors")]
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color warningColor = Color.red;

    private AltitudeManager.AltitudeState previousState;

    private Tween colorTweenText;

    private void Start()
    {
        previousState = (AltitudeManager.AltitudeState)(-1);

        UpdateVisual();
    }

    private void Update()
    {
        altitudeText.text =
            $"{drone.CurrentAltitude:0.0}m";

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (previousState == altitudeManager.SelectedState)
            return;

        previousState = altitudeManager.SelectedState;

        bool isCorrect =
            altitudeManager.SelectedState ==
            AltitudeManager.AltitudeState.Correct;

        Sprite targetFillSprite =
            isCorrect
                ? correctFillSprite
                : warningFillSprite;

        Sprite targetHandleSprite =
            isCorrect
                ? correctHandleSprite
                : warningHandleSprite;

        if (sliderFill != null)
            sliderFill.sprite = targetFillSprite;

        if (handle != null)
            handle.sprite = targetHandleSprite;

        Color targetColor =
            isCorrect
                ? correctColor
                : warningColor;

        colorTweenText?.Kill();

        if (altitudeText != null)
        {
            colorTweenText =
                altitudeText.DOColor(
                    targetColor,
                    0.2f);
        }

        if (isCorrect && handle != null)
        {
            handle.transform.DOKill();

            handle.transform
                .DOPunchScale(
                    Vector3.one * 0.15f,
                    0.3f,
                    8,
                    0.8f);
        }
    }
}