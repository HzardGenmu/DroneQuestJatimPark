using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapSelector :
    MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    public static MapSelector Instance;

    [Header("Pages")]
    [SerializeField] private RectTransform pagesRoot;
    [SerializeField] private RectTransform[] pages;

    [SerializeField] private float pageWidth = 1920f;
    [SerializeField] private float swipeThreshold = 150f;
    [SerializeField] private float pageDuration = 0.35f;

    [Header("Levels")]
    [SerializeField] private MapLevelButton[] levelButtons;

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button homeButton;

    [Header("Scenes")]
    [SerializeField] private string mainMenuScene = "MainMenu";

    [Header("Selection")]
    [SerializeField] private float playButtonScale = 1.1f;

    private int currentPage;

    private float dragStartX;

    private bool dragging;

    private MapLevelButton currentSelection;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playButton.interactable = false;

        RefreshLevels();

        PositionPagesInstant();

        playButton.onClick.AddListener(StartSelectedLevel);

        homeButton.onClick.AddListener(ReturnHome);
    }

    #region Level Selection

    public void SelectLevel(MapLevelButton level)
    {
        if (currentSelection == level)
            return;

        if (currentSelection != null)
            currentSelection.Deselect();

        currentSelection = level;

        currentSelection.Select();

        playButton.interactable = true;

        playButton.transform.DOKill();

        playButton.transform.localScale = Vector3.one;

        playButton.transform.DOKill();

        playButton.transform.localScale =
            Vector3.one;

        playButton.transform
            .DOScale(1.08f, 0.7f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    #endregion

    #region Scene Buttons

    private void StartSelectedLevel()
    {
        if (currentSelection == null)
            return;

        playButton.transform.DOKill();

        GameManager.Instance.SetCurrentLevel(
            currentSelection.Data);

        GameManager.Instance.StartLevel(
    currentSelection.Data);
    }

    private void ReturnHome()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    #endregion

    #region Swiping

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;

        dragStartX = eventData.position.x;

        pagesRoot.DOKill();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging)
            return;

        float delta =
            eventData.position.x - dragStartX;

        float x =
            -currentPage * pageWidth + delta;

        pagesRoot.anchoredPosition =
            new Vector2(
                x,
                pagesRoot.anchoredPosition.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;

        float delta =
            eventData.position.x - dragStartX;

        if (delta < -swipeThreshold &&
            currentPage < pages.Length - 1)
        {
            currentPage++;
        }

        if (delta > swipeThreshold &&
            currentPage > 0)
        {
            currentPage--;
        }

        MovePages();
    }

    #endregion

    #region Page Movement

    private void MovePages()
    {
        pagesRoot
            .DOAnchorPosX(
                -currentPage * pageWidth,
                pageDuration)
            .SetEase(Ease.OutCubic);
    }

    private void PositionPagesInstant()
    {
        pagesRoot.anchoredPosition =
            new Vector2(
                -currentPage * pageWidth,
                0f);
    }

    public void NextPage()
    {
        if (currentPage >= pages.Length - 1)
            return;

        currentPage++;

        MovePages();
    }

    public void PreviousPage()
    {
        if (currentPage <= 0)
            return;

        currentPage--;

        MovePages();
    }

    #endregion

    #region Refresh

    public void RefreshLevels()
    {
        int unlocked =
            MapSaveSystem.Instance.GetUnlockedLevel();

        foreach (MapLevelButton level in levelButtons)
        {
            bool wasLocked =
                !level.IsUnlocked;

            level.Refresh();

            if (wasLocked &&
                level.LevelIndex == unlocked)
            {
                level.PlayUnlockAnimation();
            }
        }
    }

    #endregion
}