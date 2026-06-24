using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DroneActionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DroneActionManager actionManager;

    [Header("Action Button")]
    [SerializeField] private RectTransform actionButton;
    [SerializeField] private Image actionButtonImage;

    [SerializeField] private Sprite spraySprite;
    [SerializeField] private Sprite landSprite;
    [SerializeField] private Sprite takeoffSprite;

    [Header("Spray Buttons")]
    [SerializeField] private RectTransform waterButton;
    [SerializeField] private RectTransform fertilizerButton;
    [SerializeField] private RectTransform pesticideButton;

    [SerializeField] private CanvasGroup waterCanvas;
    [SerializeField] private CanvasGroup fertilizerCanvas;
    [SerializeField] private CanvasGroup pesticideCanvas;

    [Header("Animation")]
    [SerializeField] private float animationDuration = 0.25f;
    [SerializeField] private float hiddenScale = 0.8f;

    private Vector2 waterOriginal;
    private Vector2 fertilizerOriginal;
    private Vector2 pesticideOriginal;

    private bool buttonsHidden;

    private DroneActionManager.ActionMode previousMode;

    private void Awake()
    {
        waterOriginal = waterButton.anchoredPosition;
        fertilizerOriginal = fertilizerButton.anchoredPosition;
        pesticideOriginal = pesticideButton.anchoredPosition;

        UpdateActionButtonVisual();
    }

    private void Update()
    {
        if (previousMode != actionManager.CurrentMode)
        {
            previousMode = actionManager.CurrentMode;

            UpdateActionButtonVisual();

            bool shouldHide =
                actionManager.CurrentMode ==
                DroneActionManager.ActionMode.Land || actionManager.CurrentMode == DroneActionManager.ActionMode.Takeoff;

            if (shouldHide)
                HideSprayButtons();
            else
                ShowSprayButtons();
        }
    }

    private void UpdateActionButtonVisual()
    {
        switch (actionManager.CurrentMode)
        {
            case DroneActionManager.ActionMode.Spray:
                actionButtonImage.sprite = spraySprite;
                break;

            case DroneActionManager.ActionMode.Land:
                actionButtonImage.sprite = landSprite;
                break;

            case DroneActionManager.ActionMode.Takeoff:
                actionButtonImage.sprite = takeoffSprite;
                break;
        }
    }

    private void HideSprayButtons()
    {
        if (buttonsHidden)
            return;

        buttonsHidden = true;

        Vector2 center =
            actionButton.anchoredPosition;

        AnimateButtonHide(
            waterButton,
            waterCanvas,
            center);

        AnimateButtonHide(
            fertilizerButton,
            fertilizerCanvas,
            center);

        AnimateButtonHide(
            pesticideButton,
            pesticideCanvas,
            center);
    }

    private void ShowSprayButtons()
    {
        if (!buttonsHidden)
            return;

        buttonsHidden = false;

        AnimateButtonShow(
            waterButton,
            waterCanvas,
            waterOriginal);

        AnimateButtonShow(
            fertilizerButton,
            fertilizerCanvas,
            fertilizerOriginal);

        AnimateButtonShow(
            pesticideButton,
            pesticideCanvas,
            pesticideOriginal);
    }

    private void AnimateButtonHide(
        RectTransform button,
        CanvasGroup canvas,
        Vector2 targetPosition)
    {
        button.DOKill();
        canvas.DOKill();

        button.DOAnchorPos(
            targetPosition,
            animationDuration);

        button.DOScale(
            hiddenScale,
            animationDuration);

        canvas.DOFade(
            0f,
            animationDuration);

        canvas.interactable = false;
        canvas.blocksRaycasts = false;
    }

    private void AnimateButtonShow(
        RectTransform button,
        CanvasGroup canvas,
        Vector2 originalPosition)
    {
        button.DOKill();
        canvas.DOKill();

        button.DOAnchorPos(
            originalPosition,
            animationDuration);

        button.DOScale(
            1f,
            animationDuration);

        canvas.DOFade(
            1f,
            animationDuration);

        canvas.interactable = true;
        canvas.blocksRaycasts = true;
    }
}