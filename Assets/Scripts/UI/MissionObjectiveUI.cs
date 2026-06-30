using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MissionObjectiveUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform objectivePanel;
    [SerializeField] private TMP_Text objectiveText;

    //[SerializeField] private Image glowImage;
    [SerializeField] private Image notificationBadge;

    [Header("Animation")]
    [SerializeField] private float panelDuration = 0.35f;
    [SerializeField] private float autoCollapseDelay = 6f;

    [Header("Typewriter")]
    [SerializeField] private float textDuration = 0.8f;

    private Sequence sequence;
    private Tween glowTween;
    private Tween autoCollapseTween;

    private bool expanded;
    private string currentObjective;

    private void Awake()
    {
        objectivePanel.localScale = new Vector3(0f, 1f, 1f);

        Color c = objectiveText.color;
        c.a = 0f;
        objectiveText.color = c;

        objectiveText.text = "";

        // Grab the objective from the selected level
        if (GameManager.Instance != null &&
            GameManager.Instance.CurrentLevel != null)
        {
            currentObjective =
                GameManager.Instance.CurrentLevel.objective;
        }
        else
        {
            currentObjective = "No objective assigned.";
        }

        //StartGlow();
    }

    public void Toggle()
    {
        Debug.Log("Toggle!");

        if (expanded)
            Collapse();
        else
            Expand();
    }

    private void Expand()
    {
        expanded = true;

        sequence?.Kill();
        autoCollapseTween?.Kill();

        //StopGlow();

        if (notificationBadge != null)
            notificationBadge.gameObject.SetActive(false);

        objectiveText.text = "";

        sequence = DOTween.Sequence();

        sequence.Append(
            objectivePanel
                .DOScaleX(1f, panelDuration)
                .SetEase(Ease.OutBack));

        sequence.AppendCallback(() =>
        {
            Color c = objectiveText.color;
            c.a = 1f;
            objectiveText.color = c;
        });

        sequence.AppendCallback(() =>
        {
            StartCoroutine(TypeWriter());
        });

        autoCollapseTween =
            DOVirtual.DelayedCall(
                autoCollapseDelay,
                Collapse);
    }

    private void Collapse()
    {
        expanded = false;

        sequence?.Kill();
        autoCollapseTween?.Kill();

        sequence = DOTween.Sequence();

        sequence.Append(
            objectiveText
                .DOFade(0f, 0.15f));

        sequence.Append(
            objectivePanel
                .DOScaleX(0f, panelDuration)
                .SetEase(Ease.InBack));

        sequence.AppendCallback(() =>
        {
            objectiveText.text = "";

            Color c = objectiveText.color;
            c.a = 1f;
            objectiveText.color = c;

            //StartGlow();
        });
    }

    public void SetObjective(string objective)
    {
        currentObjective = objective;
    }

    public void ShowNotification()
    {
        if (notificationBadge != null)
            notificationBadge.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        sequence?.Kill();
        glowTween?.Kill();
        autoCollapseTween?.Kill();
    }

    private IEnumerator TypeWriter()
    {
        objectiveText.text = currentObjective;
        objectiveText.maxVisibleCharacters = 0;

        while (objectiveText.maxVisibleCharacters <
               currentObjective.Length)
        {
            objectiveText.maxVisibleCharacters++;

            yield return new WaitForSeconds(
                textDuration /
                currentObjective.Length);
        }
    }
}