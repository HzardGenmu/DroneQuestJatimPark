using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionObjectiveUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject objectivePanel;

    [SerializeField] private Image notificationBadge;

    [Header("Water")]
    [SerializeField] private Slider waterSlider;
    [SerializeField] private TMP_Text waterText;

    [Header("Fertilizer")]
    [SerializeField] private Slider fertilizerSlider;
    [SerializeField] private TMP_Text fertilizerText;

    [Header("Pesticide")]
    [SerializeField] private Slider pesticideSlider;
    [SerializeField] private TMP_Text pesticideText;

    private bool expanded;

    private void Awake()
    {
        objectivePanel.SetActive(false);

        if (notificationBadge != null)
            notificationBadge.gameObject.SetActive(false);
    }

    public void Toggle()
    {
        if (expanded)
            Close();
        else
            Open();
    }

    public void Open()
    {
        expanded = true;

        objectivePanel.SetActive(true);

        if (notificationBadge != null)
            notificationBadge.gameObject.SetActive(false);
    }

    public void Close()
    {
        expanded = false;

        objectivePanel.SetActive(false);
    }

    public void ShowNotification()
    {
        if (!expanded && notificationBadge != null)
            notificationBadge.gameObject.SetActive(true);
    }

    public void UpdateTreatmentProgress(
        CropTreatment treatment,
        int completed,
        int total)
    {
        Slider slider = null;
        TMP_Text label = null;

        switch (treatment)
        {
            case CropTreatment.Water:
                slider = waterSlider;
                label = waterText;
                break;

            case CropTreatment.Fertilizer:
                slider = fertilizerSlider;
                label = fertilizerText;
                break;

            case CropTreatment.Pesticide:
                slider = pesticideSlider;
                label = pesticideText;
                break;
        }

        if (slider != null)
        {
            slider.maxValue = Mathf.Max(total, 1);
            slider.value = completed;
        }

        if (label != null)
        {
            label.text = $"{completed}/{total}";
        }
    }
}