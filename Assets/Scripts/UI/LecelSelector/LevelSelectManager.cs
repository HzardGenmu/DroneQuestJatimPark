using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    [SerializeField] private LevelEntry[] levels;

    [SerializeField] private float pageWidth = 1920f;
    [SerializeField] private float duration = 0.4f;
    [SerializeField] private float swipeThreshold = 150f;
    [SerializeField] private RectTransform pageContainer;

    [SerializeField]
    private string mainMenuScene = "MainMenu";

    private int currentPage;
    private bool dragging;
    public LevelData CurrentLevel => levels[currentPage].data;
    public int CurrentPage => currentPage;
    public int LevelCount => levels.Length;

    private void Start()
    {
        pageWidth = pageContainer.rect.width;

        MovePagesInstant();
    }

    public void NextPage()
    {
        if (currentPage >= levels.Length - 1)
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

    public void BeginDrag()
    {
        dragging = true;

        foreach (LevelEntry level in levels)
        {
            level.page.Root.DOKill();

            level.page.Background.DOKill();
            level.page.Midground.DOKill();
            level.page.Foreground.DOKill();
            level.page.UI.DOKill();
        }
    }

    public void Drag(float delta)
    {
        if (!dragging)
            return;

        bool firstPage =
            currentPage == 0;

        bool lastPage =
            currentPage == levels.Length - 1;

        float move = delta;

        // Rubber-band effect
        if ((firstPage && delta > 0) ||
            (lastPage && delta < 0))
        {
            move *= 0.3f;
        }

        foreach (LevelEntry level in levels)
        {
            level.page.Root.anchoredPosition +=
                Vector2.right * move;

            level.page.Background.anchoredPosition +=
                Vector2.right * move * 0.6f;

            level.page.Midground.anchoredPosition +=
                Vector2.right * move;

            level.page.Foreground.anchoredPosition +=
                Vector2.right * move * 1.4f;

            level.page.UI.anchoredPosition +=
                Vector2.right * move * 1.1f;
        }
    }

    public void EndDrag(float dragDistance)
    {
        dragging = false;

        if (dragDistance < -swipeThreshold &&
            currentPage < levels.Length - 1)
        {
            currentPage++;
        }
        else if (dragDistance > swipeThreshold &&
                 currentPage > 0)
        {
            currentPage--;
        }

        MovePages();
    }

    private void MovePages()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            float x =
                (i - currentPage) * pageWidth;

            levels[i].page.Root
                .DOAnchorPos(
                    new Vector2(x, 0),
                    duration)
                .SetEase(Ease.OutQuart);

            levels[i].page.Background
                .DOAnchorPos(
                    levels[i].page.BackgroundStart + Vector2.right * x * 0.6f,
                    duration);

            levels[i].page.Midground
                .DOAnchorPos(
                    levels[i].page.MidgroundStart + Vector2.right * x,
                    duration);

            levels[i].page.Foreground
                .DOAnchorPos(
                    levels[i].page.ForegroundStart + Vector2.right * x * 1.4f,
                    duration);

            levels[i].page.UI
                .DOAnchorPos(
                    levels[i].page.UIStart + Vector2.right * x * 1.1f,
                    duration);
        }
    }

    private void MovePagesInstant()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            float x =
                (i - currentPage) * pageWidth;

            levels[i].page.Root.anchoredPosition =
                new Vector2(x, 0);

            levels[i].page.Background.anchoredPosition =
                levels[i].page.BackgroundStart + Vector2.right * x * 0.6f;
            levels[i].page.Midground.anchoredPosition =
                levels[i].page.MidgroundStart + Vector2.right * x;

            levels[i].page.Foreground.anchoredPosition =
                levels[i].page.ForegroundStart + Vector2.right * x * 1.4f;  
            levels[i].page.UI.anchoredPosition =
                levels[i].page.UIStart + Vector2.right * x * 1.1f;
        }
    }

    public void GoToPage(int index)
    {
        index = Mathf.Clamp(index, 0, levels.Length - 1);

        if (index == currentPage)
            return;

        currentPage = index;

        MovePages();
    }

    public void StartLevel()
    {
        Debug.Log(CurrentLevel.sceneName);
        SceneManager.LoadScene(CurrentLevel.sceneName);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}