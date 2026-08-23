using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    public static TutorialUI Instance;

    [Header("Canvas")]
    public RectTransform canvasRect;

    [Header("Main")]
    public GameObject panel;

    public RectTransform contentRoot;

    public TMP_Text descriptionText;
    [SerializeField] private RectTransform contentContainer;

    [Header("Overlay")]
    public GameObject darkOverlay;

    [Header("Panel")]
    [SerializeField] private RectTransform panelGraphic;

    [SerializeField]
    private TutorialSpotlightOverlay spotlightOverlay;

    [Header("Buttons")]
    [SerializeField] private GameObject previousButton;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private RectTransform buttonsRoot;

    [Header("Prompt Backgrounds")]
    [SerializeField] private GameObject upBackground;
    [SerializeField] private GameObject downBackground;
    [SerializeField] private GameObject leftBackground;
    [SerializeField] private GameObject rightBackground;

    private readonly List<Canvas> elevatedCanvases =
    new List<Canvas>();

    private readonly Dictionary<Canvas, int>
        originalSortingOrders =
            new Dictionary<Canvas, int>();

    private readonly Dictionary<Canvas, bool>
        originalOverrideStates =
            new Dictionary<Canvas, bool>();

    private readonly Dictionary<Canvas, bool> createdCanvas =
    new Dictionary<Canvas, bool>();

    private TutorialStep activeStep;

    private void Awake()
    {
        Instance = this;

        panel.SetActive(false);
        darkOverlay.SetActive(false);

        SetupButtonsOnTop();
    }

    private void Update()
    {
        if (!panel.activeSelf)
            return;
    }
    public void Show(TutorialStep step)
    {
        ClearElevatedUI();

        panel.SetActive(true);

        descriptionText.text = step.description;

        UpdatePromptBackground(step.panelAnchor);

        FlipPanel(step.flipPanel);

        if (step.showButtons)
        {
            previousButton.SetActive(
                TutorialManager.Instance.CurrentStepIndex > 0
            );

            nextButton.SetActive(true);
        }
        else
        {
            previousButton.SetActive(false);
            nextButton.SetActive(false);
        }

        PositionPanel(step);

        activeStep = step;


        bool hasFocus =
            step.focusItems.Count > 0;

        darkOverlay.SetActive(hasFocus);

        if (spotlightOverlay != null)
        {
            spotlightOverlay.ClearHoles();
        }

        int holeIndex = 0;

        foreach (var focusData in step.focusItems)
        {
            List<Transform> targets =
                TutorialTargetRegistry.Instance
                    .GetTargets(
                        focusData.targetID
                    );

            foreach (Transform target in targets)
            {
                RectTransform uiRect =
                    target as RectTransform;

                if (uiRect != null)
                {
                    ElevateUIElement(target);

                    Debug.Log(
                        $"Elevated UI {target.name}"
                    );

                    continue;
                }

                spotlightOverlay.TrackTarget(
                    holeIndex,
                    target,
                    focusData.highlightSize,
                    focusData.autoSize,
                    focusData.highlightOffset
                );

                Debug.Log(
                    $"Tracking World Object {target.name}"
                );

                holeIndex++;
            }
        }
    }

    void PositionPanel(TutorialStep step)
    {
        if (contentRoot == null)
            return;

        RectTransform panelRect =
            contentRoot;

        RectTransform canvas =
            canvasRect;

        Vector2 position =
            Vector2.zero;

        bool hasFocus =
            step.focusItems != null &&
            step.focusItems.Count > 0;

        if (hasFocus)
        {
            List<Transform> targets =
                TutorialTargetRegistry.Instance
                    .GetTargets(
                        step.focusItems[0].targetID
                    );

            if (targets.Count > 0)
            {
                Transform target =
                    targets[0];

                Vector2 targetPosition =
                    GetTargetCanvasPosition(
                        target
                    );

                float spacing = 250f;

                switch (step.panelAnchor)
                {
                    case TutorialPanelAnchor.Top:
                        position =
                            targetPosition +
                            Vector2.up * spacing;
                        break;

                    case TutorialPanelAnchor.Bottom:
                        position =
                            targetPosition +
                            Vector2.down * spacing;
                        break;

                    case TutorialPanelAnchor.Left:
                        position =
                            targetPosition +
                            Vector2.left * spacing;
                        break;

                    case TutorialPanelAnchor.Right:
                        position =
                            targetPosition +
                            Vector2.right * spacing;
                        break;

                    default:
                        position =
                            targetPosition;
                        break;
                }
            }
        }

        position += step.panelOffset;

        position =
            ClampToCanvas(
                position,
                panelRect,
                canvas
            );

        panelRect.anchoredPosition =
            position;
    }

    Vector2 GetTargetCanvasPosition(
    Transform target)
    {
        RectTransform canvas =
            canvasRect;

        Vector2 localPoint =
            Vector2.zero;

        if (target is RectTransform rect)
        {
            Vector2 screenPos =
                RectTransformUtility
                    .WorldToScreenPoint(
                        null,
                        rect.position
                    );

            RectTransformUtility
                .ScreenPointToLocalPointInRectangle(
                    canvas,
                    screenPos,
                    null,
                    out localPoint
                );
        }
        else
        {
            Camera cam =
                Camera.main;

            Vector2 screenPos =
                cam.WorldToScreenPoint(
                    target.position
                );

            RectTransformUtility
                .ScreenPointToLocalPointInRectangle(
                    canvas,
                    screenPos,
                    null,
                    out localPoint
                );
        }

        return localPoint;
    }

    Vector2 AutoFlipPosition(
    TutorialPanelAnchor anchor,
    Vector2 targetPosition,
    float spacing)
    {
        Rect canvasRectData =
            canvasRect.rect;

        switch (anchor)
        {
            case TutorialPanelAnchor.Right:

                if (
                    targetPosition.x >
                    canvasRectData.width * 0.25f
                )
                {
                    return targetPosition +
                           Vector2.left * spacing;
                }

                break;

            case TutorialPanelAnchor.Left:

                if (
                    targetPosition.x <
                    -canvasRectData.width * 0.25f
                )
                {
                    return targetPosition +
                           Vector2.right * spacing;
                }

                break;

            case TutorialPanelAnchor.Top:

                if (
                    targetPosition.y >
                    canvasRectData.height * 0.25f
                )
                {
                    return targetPosition +
                           Vector2.down * spacing;
                }

                break;

            case TutorialPanelAnchor.Bottom:

                if (
                    targetPosition.y <
                    -canvasRectData.height * 0.25f
                )
                {
                    return targetPosition +
                           Vector2.up * spacing;
                }

                break;
        }

        switch (anchor)
        {
            case TutorialPanelAnchor.Top:
                return targetPosition +
                       Vector2.up * spacing;

            case TutorialPanelAnchor.Bottom:
                return targetPosition +
                       Vector2.down * spacing;

            case TutorialPanelAnchor.Left:
                return targetPosition +
                       Vector2.left * spacing;

            case TutorialPanelAnchor.Right:
                return targetPosition +
                       Vector2.right * spacing;
        }

        return targetPosition;
    }

    Vector2 ClampToCanvas(
    Vector2 position,
    RectTransform panel,
    RectTransform canvas)
    {
        Vector2 panelSize =
            panel.rect.size;

        Rect canvasRectData =
            canvas.rect;

        float halfWidth =
            panelSize.x * 0.5f;

        float halfHeight =
            panelSize.y * 0.5f;

        position.x =
            Mathf.Clamp(
                position.x,
                canvasRectData.xMin + halfWidth,
                canvasRectData.xMax - halfWidth
            );

        position.y =
            Mathf.Clamp(
                position.y,
                canvasRectData.yMin + halfHeight,
                canvasRectData.yMax - halfHeight
            );

        return position;
    }


    public void Hide()
    {
        panel.SetActive(false);

        darkOverlay.SetActive(false);

        if (spotlightOverlay != null)
        {
            spotlightOverlay.ClearHoles();
        }

        ClearElevatedUI();
    }

    //void ClearFocuses()
    //{
    //    foreach (var focus in activeFocuses)
    //    {
    //        Destroy(focus.gameObject);
    //    }

    //    activeFocuses.Clear();
    //}

    public void OnContinueButton()
    {
        Debug.Log("Continue button clicked");
        if (TutorialManager.Instance != null)
        {
            Debug.Log("Continuing tutorial");
            TutorialManager.Instance.ContinueTutorial();
        }
    }

    public void OnSkipButton()
    {
        if (!panel.activeSelf)
            return;

        TutorialManager.Instance
            .SkipTutorial();
    }

    public void OnTutorialSubmit(
    InputAction.CallbackContext ctx
)
    {
        if (!ctx.performed)
            return;

        OnContinueButton();
    }

    public void OnPreviousButton()
    {
        Debug.Log("Previous button clicked");

        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.PreviousStep();
        }
    }

    private void ClearElevatedUI()
    {
        //foreach (Canvas canvas in elevatedCanvases)
        //{
        //    if (canvas == null)
        //        continue;

        //    if (originalSortingOrders.TryGetValue(
        //            canvas,
        //            out int order))
        //    {
        //        canvas.sortingOrder = order;
        //    }

        //    if (originalOverrideStates.TryGetValue(
        //            canvas,
        //            out bool overrideSorting))
        //    {
        //        canvas.overrideSorting =
        //            overrideSorting;
        //    }
        //}
        foreach (Canvas canvas in elevatedCanvases)
        {
            if (createdCanvas[canvas])
            {
                Destroy(canvas);
            }
            else
            {
                canvas.overrideSorting = originalOverrideStates[canvas];
                canvas.sortingOrder = originalSortingOrders[canvas];
            }
        }

        elevatedCanvases.Clear();
        originalSortingOrders.Clear();
        originalOverrideStates.Clear();
    }

    private void UpdatePromptBackground(TutorialPanelAnchor anchor)
    {
        if (upBackground != null)
            upBackground.SetActive(false);

        if (downBackground != null)
            downBackground.SetActive(false);

        if (leftBackground != null)
            leftBackground.SetActive(false);

        if (rightBackground != null)
            rightBackground.SetActive(false);

        switch (anchor)
        {
            case TutorialPanelAnchor.Top:

                if (upBackground != null)
                    upBackground.SetActive(true);

                break;

            case TutorialPanelAnchor.Bottom:

                if (downBackground != null)
                    downBackground.SetActive(true);

                break;

            case TutorialPanelAnchor.Left:

                if (leftBackground != null)
                    leftBackground.SetActive(true);

                break;

            case TutorialPanelAnchor.Right:

                if (rightBackground != null)
                    rightBackground.SetActive(true);

                break;

            case TutorialPanelAnchor.Center:

                // No directional background.
                break;
        }
    }

    private void SetupButtonsOnTop()
    {
        if (buttonsRoot == null)
            return;

        Canvas canvas =
            buttonsRoot.GetComponent<Canvas>();

        if (canvas == null)
            canvas =
                buttonsRoot.gameObject.AddComponent<Canvas>();

        canvas.overrideSorting = true;

        // Higher than the highlighted UI.
        canvas.sortingOrder = 2000;

        // Make sure the nested Canvas can receive UI input.
        GraphicRaycaster raycaster =
            buttonsRoot.GetComponent<GraphicRaycaster>();

        if (raycaster == null)
            buttonsRoot.gameObject.AddComponent<GraphicRaycaster>();
    }

    private void ElevateUIElement(
        Transform target)
    {
        if (target == null)
            return;

        Canvas canvas = target.GetComponent<Canvas>();

        bool wasCreated = false;

        if (canvas == null)
        {
            canvas = target.gameObject.AddComponent<Canvas>();
            wasCreated = true;
        }

        createdCanvas[canvas] = wasCreated;

        if (!elevatedCanvases.Contains(canvas))
        {
            elevatedCanvases.Add(canvas);

            originalSortingOrders[canvas] =
                canvas.sortingOrder;

            originalOverrideStates[canvas] =
                canvas.overrideSorting;
        }

        canvas.overrideSorting = true;
        canvas.sortingOrder = 1000;
    }
    private void FlipPanel(bool flip)
    {
        // Flip panel
        Vector3 panelScale = panelGraphic.localScale;
        panelScale.x = Mathf.Abs(panelScale.x) * (flip ? -1 : 1);
        panelGraphic.localScale = panelScale;

        // Flip content back
        Vector3 contentScale = contentContainer.localScale;
        contentScale.x = Mathf.Abs(contentScale.x) * (flip ? -1 : 1);
        contentContainer.localScale = contentScale;
    }
}