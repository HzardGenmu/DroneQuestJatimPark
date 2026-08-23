using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Main Action Button")]
    [SerializeField] private Image mainActionRing;

    [Header("Spray Selection")]
    [SerializeField] private DroneSprayer sprayer;

    [SerializeField] private Image waterRing;
    [SerializeField] private Image fertilizerRing;
    [SerializeField] private Image pesticideRing;

    [SerializeField] private bool needTutorial = false;
    [SerializeField] private float tutorialStartDelay = 3f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RefreshSpraySelection();
        SetMainActionActive(false);

        if (needTutorial)
        {
            StartCoroutine(StartTutorialAfterDelay());
        }
    }

    private IEnumerator StartTutorialAfterDelay()
    {
        yield return new WaitForSeconds(tutorialStartDelay);

        CheckTutorial();
    }

    #region Main Action Button

    public void MainActionPressed()
    {
        SetMainActionActive(true);
    }

    public void MainActionReleased()
    {
        SetMainActionActive(false);
    }

    private void SetMainActionActive(bool active)
    {
        if (mainActionRing != null)
            mainActionRing.enabled = active;
    }

    #endregion

    #region Spray Selection

    public void SelectWater()
    {
        sprayer.SelectWater();
        RefreshSpraySelection();
    }

    public void SelectFertilizer()
    {
        sprayer.SelectFertilizer();
        RefreshSpraySelection();
    }

    public void SelectPesticide()
    {
        sprayer.SelectPesticide();
        RefreshSpraySelection();
    }

    private void RefreshSpraySelection()
    {
        if (sprayer == null)
            return;

        waterRing.enabled =
            sprayer.CurrentSprayType == SprayType.Water;

        fertilizerRing.enabled =
            sprayer.CurrentSprayType == SprayType.Fertilizer;

        pesticideRing.enabled =
            sprayer.CurrentSprayType == SprayType.Pesticide;
    }

    #endregion

    private void CheckTutorial()
    {
        //if (!TutorialSave.IsCompleted())
        //{
        //    TutorialManager.Instance.StartTutorial();
        //}
        TutorialManager.Instance.StartTutorial();
    }
}