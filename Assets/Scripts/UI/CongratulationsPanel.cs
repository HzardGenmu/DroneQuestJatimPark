using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CongratulationsPanel : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private RectTransform panel;

    [Header("Buttons")]
    [SerializeField] private RectTransform[] buttons;

    [Header("Timing")]
    [SerializeField] private float buttonDelay = 0.12f;
    [SerializeField] private float buttonDuration = 0.35f;

    [Header("Scenes")]
    [SerializeField] private string mapScene = "MapSelector";
    [SerializeField] private string menuScene = "MainMenu";

    private void Awake()
    {
        gameObject.SetActive(false);

        foreach (RectTransform button in buttons)
        {
            button.localScale = Vector3.zero;
            button.gameObject.SetActive(false);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);

        // Your Animator plays here automatically.
        // Add an Animation Event at the end of the animation
        // that calls OnPanelRevealFinished().
    }

    // Called from the last frame of the Animator animation.
    public void OnPanelRevealFinished()
    {
        Sequence revealSequence = DOTween.Sequence();

        foreach (RectTransform button in buttons)
        {
            revealSequence.AppendCallback(() =>
            {
                button.gameObject.SetActive(true);

                button.localScale = Vector3.zero;

                button.DOScale(1f, buttonDuration)
                    .SetEase(Ease.OutBack);

                button.DOShakeRotation(
                    0.28f,
                    new Vector3(0, 0, 12f),
                    12,
                    90f);
            });

            revealSequence.AppendInterval(buttonDelay);
        }

        revealSequence.OnComplete(StartIdleAnimations);
    }

    private void StartIdleAnimations()
    {
        //-----------------------------------
        // Buttons pulse
        //-----------------------------------

        foreach (RectTransform button in buttons)
        {
            button.DOScale(1.08f, 0.9f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetDelay(Random.Range(0f, 0.35f));
        }

        //-----------------------------------
        // Panel bob
        //-----------------------------------

        panel.DOLocalMoveY(
                panel.localPosition.y + 10f,
                2.1f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void GoToMap()
    {
        SceneManager.LoadScene(mapScene);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(menuScene);
    }
}