using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignalStaticEffect : MonoBehaviour
{
    [Header("Static")]
    [SerializeField] private Image staticImage;

    [Header("Warning")]
    [SerializeField] private CanvasGroup warningCanvas;
    [SerializeField] private TMP_Text warningText;

    private bool warningVisible;

    public void SetIntensity(float value)
    {
        Color c = staticImage.color;

        c.a =
            value *
            Random.Range(
                0.7f,
                1f);

        staticImage.color = c;
    }

    public void ShowWarning()
    {
        if (warningVisible)
            return;

        warningVisible = true;

        warningCanvas.DOFade(
            1f,
            0.25f);

        warningText.text =
            "SIGNAL WEAK\nRETURN TO OPERATION AREA";
    }

    public void HideWarning()
    {
        if (!warningVisible)
            return;

        warningVisible = false;

        warningCanvas.DOFade(
            0f,
            0.25f);
    }
}