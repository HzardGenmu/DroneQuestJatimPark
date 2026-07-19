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
    //[SerializeField] private Slider altitudeSlider;
    [SerializeField] private Image sliderFill;
    [SerializeField] private Image handle;
    [SerializeField] private TMP_Text altitudeText;

    [Header("Colors")]
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color warningColor = Color.red;

    private AltitudeManager.AltitudeState previousState;
    private Tween colorTweenFill;
    private Tween colorTweenHandle;
    private Tween colorTweenText;

    private void Update()
    {
        altitudeText.text =
            $"{drone.CurrentAltitude:0.0}m";

        UpdateColor();
    }

    //private void UpdateAltitude()
    //{
    //    float altitude = drone.CurrentAltitude;

    //    float normalized =
    //        Mathf.InverseLerp(
    //            drone.MinAltitude,
    //            drone.MaxAltitude,
    //            altitude);

    //    if (Mathf.Abs(normalized - lastSliderValue) > 0.002f)
    //    {
    //        lastSliderValue = normalized;

    //        altitudeSlider.value = normalized;

    //        altitudeText.text = $"{altitude:0.0}m";
    //    }
    //}

    private void UpdateColor()
    {
        if (previousState == altitudeManager.CurrentState)
            return;

        previousState = altitudeManager.CurrentState;

        Color targetColor =
            altitudeManager.CurrentState ==
            AltitudeManager.AltitudeState.Correct
                ? correctColor
                : warningColor;

        colorTweenFill?.Kill();
        colorTweenHandle?.Kill();
        colorTweenText?.Kill();

        colorTweenFill =
            sliderFill.DOColor(targetColor, 0.2f);

        colorTweenHandle =
            handle.DOColor(targetColor, 0.2f);

        colorTweenText =
            altitudeText.DOColor(targetColor, 0.2f);

        if (altitudeManager.CurrentState ==
            AltitudeManager.AltitudeState.Correct)
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

    //public void WrongAltitudeFeedback()
    //{
    //    altitudeSlider.transform.DOKill();

    //    altitudeSlider.transform
    //        .DOShakePosition(
    //            0.25f,
    //            6f,
    //            20);
    //}
}