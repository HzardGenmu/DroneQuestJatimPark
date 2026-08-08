using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelResultPanel : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private RectTransform fanfare;
    [SerializeField] private Image fanfareImage;

    [Header("Fanfare Sprites")]
    [SerializeField] private Sprite completeFanfareSprite;
    [SerializeField] private Sprite failedFanfareSprite;

    [Header("Result Image")]
    [SerializeField] private Image resultImage;
    [SerializeField] private Sprite completeSprite;
    [SerializeField] private Sprite failedSprite;

    [Header("Budi")]
    [SerializeField] private Image budi;
    [SerializeField] private RectTransform budiTransform;
    [SerializeField] private Sprite completeBudiSprite;
    [SerializeField] private Sprite failedBudiSprite;

    [Header("Stars")]
    [SerializeField] private RectTransform[] emptyStars;
    [SerializeField] private RectTransform[] filledStars;

    [SerializeField] private float starDelay = 0.25f;
    [SerializeField] private float starDuration = 0.35f;

    [Header("Buttons")]
    [SerializeField] private RectTransform retryButton;
    [SerializeField] private RectTransform continueButton;
    [SerializeField] private RectTransform mapButton;

    [Header("Timing")]
    [SerializeField] private float buttonDelay = 0.12f;
    [SerializeField] private float buttonDuration = 0.35f;

    [Header("Scenes")]
    [SerializeField] private string mapScene = "MapSelector";

    private RectTransform[] visibleButtons;
    private int earnedStars;
    private Sequence revealSequence;

    private void Awake()
    {
        gameObject.SetActive(false);

        retryButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        mapButton.gameObject.SetActive(false);

        budiTransform.localScale = Vector3.zero;
        budi.gameObject.SetActive(false);

        fanfare.localScale = Vector3.zero;
        fanfare.gameObject.SetActive(false);

        foreach (RectTransform star in emptyStars)
        {
            star.localScale = Vector3.zero;
            star.gameObject.SetActive(false);
        }

        foreach (RectTransform star in filledStars)
        {
            star.localScale = Vector3.zero;
            star.gameObject.SetActive(false);
        }

        retryButton.localScale = Vector3.zero;
        continueButton.localScale = Vector3.zero;
        mapButton.localScale = Vector3.zero;

        fanfare.localRotation = Quaternion.identity;
    }

    public void Show(LevelResult result, int starsEarned)
    {
        earnedStars = starsEarned;

        gameObject.SetActive(true);

        revealSequence?.Kill();

        DOTween.Kill(panel);
        DOTween.Kill(fanfare);
        DOTween.Kill(budiTransform);

        //----------------------------------------
        // Result Images
        //----------------------------------------

        resultImage.sprite =
            result == LevelResult.Complete
            ? completeSprite
            : failedSprite;

        fanfareImage.sprite =
            result == LevelResult.Complete
            ? completeFanfareSprite
            : failedFanfareSprite;

        budi.sprite =
            result == LevelResult.Complete
            ? completeBudiSprite
            : failedBudiSprite;

        //----------------------------------------
        // Reset Budi
        //----------------------------------------

        budi.gameObject.SetActive(false);

        budiTransform.localScale = Vector3.zero;

        DOTween.Kill(budiTransform);

        //----------------------------------------
        // Reset Fanfare
        //----------------------------------------

        fanfare.gameObject.SetActive(false);

        fanfare.localScale = Vector3.zero;
        fanfare.localRotation = Quaternion.identity;

        DOTween.Kill(fanfare);

        //----------------------------------------
        // Reset Stars
        //----------------------------------------

        for (int i = 0; i < emptyStars.Length; i++)
        {
            DOTween.Kill(emptyStars[i]);
            DOTween.Kill(filledStars[i]);

            emptyStars[i].gameObject.SetActive(false);
            filledStars[i].gameObject.SetActive(false);

            emptyStars[i].localScale = Vector3.zero;
            filledStars[i].localScale = Vector3.zero;

            emptyStars[i].localRotation = Quaternion.identity;
            filledStars[i].localRotation = Quaternion.identity;
        }

        //----------------------------------------
        // Reset Buttons
        //----------------------------------------

        retryButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        mapButton.gameObject.SetActive(false);

        retryButton.localScale = Vector3.zero;
        continueButton.localScale = Vector3.zero;
        mapButton.localScale = Vector3.zero;

        //----------------------------------------
        // Which buttons?
        //----------------------------------------

        if (result == LevelResult.Complete)
        {
            visibleButtons = new[]
            {
            continueButton,
            mapButton
        };
        }
        else
        {
            visibleButtons = new[]
            {
            retryButton,
            mapButton
        };
        }

        foreach (RectTransform button in visibleButtons)
            DOTween.Kill(button);
    }

    public void OnPanelRevealFinished()
    {
        revealSequence?.Kill();

        revealSequence = DOTween.Sequence();

        //------------------------------------
        // BUDI
        //------------------------------------

        revealSequence.AppendCallback(() =>
        {
            budi.gameObject.SetActive(true);

            budiTransform.localScale = Vector3.zero;

            budiTransform
                .DOScale(1f, .45f)
                .SetEase(Ease.OutBack);

            budiTransform
                .DOShakeRotation(
                    .35f,
                    new Vector3(0, 0, 12),
                    18,
                    90f);
        });

        revealSequence.AppendInterval(.35f);

        //------------------------------------
        // FANFARE
        //------------------------------------

        revealSequence.AppendCallback(() =>
        {
            fanfare.gameObject.SetActive(true);

            fanfare.localScale = Vector3.zero;

            fanfare
                .DOScale(1f, .45f)
                .SetEase(Ease.OutBack);

            fanfare
                .DORotate(
                    new Vector3(0, 0, 360),
                    8f,
                    RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1);
        });

        revealSequence.AppendInterval(.4f);

        //------------------------------------
        // EMPTY STARS
        //------------------------------------

        revealSequence.AppendCallback(() =>
        {
            foreach (RectTransform star in emptyStars)
            {
                star.gameObject.SetActive(true);

                star.localScale = Vector3.zero;

                Sequence s = DOTween.Sequence();

                s.Append(
                    star
                        .DOScale(1.15f, .2f)
                        .SetEase(Ease.OutBack));

                s.Append(
                    star
                        .DOScale(1f, .1f));
            }
        });

        revealSequence.AppendInterval(.4f);

        //------------------------------------
        // FILLED STARS
        //------------------------------------

        for (int i = 0; i < earnedStars; i++)
        {
            int index = i;

            revealSequence.AppendCallback(() =>
            {
                filledStars[index].gameObject.SetActive(true);

                filledStars[index].localScale = Vector3.zero;
                filledStars[index].localRotation = Quaternion.identity;

                Sequence starSeq = DOTween.Sequence();

                starSeq.Append(
                    filledStars[index]
                        .DOScale(1.25f, starDuration * .5f)
                        .SetEase(Ease.OutBack));

                starSeq.Join(
                    filledStars[index]
                        .DORotate(
                            new Vector3(0, 0, 360),
                            starDuration,
                            RotateMode.FastBeyond360));

                starSeq.Append(
                    filledStars[index]
                        .DOScale(1f, starDuration * .5f));
            });

            revealSequence.AppendInterval(starDelay);
        }

        //------------------------------------
        // BUTTONS
        //------------------------------------

        foreach (RectTransform button in visibleButtons)
        {
            revealSequence.AppendCallback(() =>
            {
                button.gameObject.SetActive(true);

                button.localScale = Vector3.zero;

                button
                    .DOScale(1f, buttonDuration)
                    .SetEase(Ease.OutBack);

                button
                    .DOShakeRotation(
                        .25f,
                        new Vector3(0, 0, 10),
                        12,
                        90f);
            });

            revealSequence.AppendInterval(buttonDelay);
        }

        revealSequence.OnComplete(StartIdleAnimations);
    }

    private void StartIdleAnimations()
    {
        foreach (RectTransform button in visibleButtons)
        {
            button
                .DOScale(1.08f, .9f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetDelay(Random.Range(0f, .35f));
        }

        panel
            .DOLocalMoveY(
                panel.localPosition.y + 10f,
                2f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    //----------------------------------------
    // Buttons
    //----------------------------------------

    public void Retry()
    {
        if (GameManager.Instance.CurrentLevel != null)
        {
            GameManager.Instance.ChangeState(
                GameState.Gameplay,
                GameManager.Instance.CurrentLevel);
        }
    }

    public void Continue()
    {
        GameManager.Instance.ChangeState(GameState.MapSelect);
    }

    public void BackToMap()
    {
        GameManager.Instance.ChangeState(GameState.MapSelect);
    }
}