using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DroneDispatchAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DroneDisplayManager displayManager;
    [SerializeField] private DroneViewer viewer;

    [SerializeField] private CanvasGroup menuCanvas;
    [SerializeField] private Image fadeImage;

    [SerializeField] private Transform dispatchTarget;

    [Header("Scene")]
    [SerializeField] private string gameplayScene = "LevelSelector";

    [Header("Animation")]
    [SerializeField] private float rotateDuration = 0.4f;
    [SerializeField] private float takeoffHeight = 1.5f;
    [SerializeField] private float takeoffDuration = 0.45f;
    [SerializeField] private float flyDuration = 1.2f;
    [SerializeField] private float fadeDuration = 0.5f;

    private bool dispatching;

    private void Start()
    {
        if (fadeImage == null)
            return;

        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;

        fadeImage.gameObject.SetActive(true);

        fadeImage
            .DOFade(0f, fadeDuration)
            .OnComplete(() =>
            {
                fadeImage.gameObject.SetActive(false);
            });
    }

    public void BeginDispatch()
    {
        if (dispatching)
            return;

        dispatching = true;

        if (viewer != null)
            viewer.enabled = false;

        if (menuCanvas != null)
        {
            menuCanvas.interactable = false;
            menuCanvas.blocksRaycasts = false;
        }

        GameObject drone =
            displayManager.CurrentDrone;

        DroneIdle idle =
            drone.GetComponent<DroneIdle>();

        if (idle != null)
            idle.EnableIdle = false;

        drone.transform.DOKill();

        Sequence sequence =
            DOTween.Sequence();

        // Face forward first
        sequence.Append(
            drone.transform.DORotate(
                Vector3.zero,
                rotateDuration)
            .SetEase(Ease.OutSine));

        // Lift off
        sequence.Append(
            drone.transform.DOMoveY(
                drone.transform.position.y + takeoffHeight,
                takeoffDuration)
            .SetEase(Ease.OutQuad));

        // Fly away
        sequence.Append(
            drone.transform.DOMove(
                dispatchTarget.position,
                flyDuration)
            .SetEase(Ease.InQuad));

        sequence.Join(
            drone.transform.DOScale(
                0.7f,
                flyDuration));

        sequence.AppendCallback(() =>
        {
            fadeImage.gameObject.SetActive(true);
        });

        sequence.Append(
            fadeImage
                .DOFade(1f, fadeDuration));

        sequence.OnComplete(() =>
        {
            SceneManager.LoadScene(gameplayScene);
        });
    }
}