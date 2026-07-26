using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField]
    private List<TutorialStep> steps;

    [SerializeField] private GameObject tutorialSetup;

    private int currentStepIndex = -1;
    private static int pauseCount = 0;

    private bool isRunning = false;
    private bool tutorialPausedGame = false;
    private bool isTransitioningStep = false;
    private CropField firstScannedPlant;
    private CropField firstTreatedPlant;
    private bool tutorialActive => isRunning && currentStepIndex >= 0 && currentStepIndex < steps.Count;

    public TutorialStep CurrentStep =>
        currentStepIndex >= 0 &&
        currentStepIndex < steps.Count
            ? steps[currentStepIndex]
            : null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        GameEvents.OnPlantScanned += HandlePlantScanned;
        GameEvents.OnPlantTreated += HandlePlantTreated;
        GameEvents.OnCameraModeChanged += HandleCameraModeChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnPlantScanned -= HandlePlantScanned;
        GameEvents.OnPlantTreated -= HandlePlantTreated;
        GameEvents.OnCameraModeChanged -= HandleCameraModeChanged;
    }

    public void StartTutorial()
    {
        //if (
        //    TutorialSave.IsCompleted()
        //    || TutorialSave.HasStarted()
        //)
        //{
        //    gameObject.SetActive(false);
        //    return;
        //}

        tutorialSetup.SetActive(true);
        TutorialSave.SetStarted();

        isRunning = true;

        NextStep();
    }

    public void SkipTutorial()
    {
        Debug.Log("Tutorial Skipped");

        EndTutorial();
    }

    void EndTutorial()
    {
        isRunning = false;

        if (tutorialPausedGame)
        {
            Resume();
            tutorialPausedGame = false;
        }

        TutorialSave.SetCompleted();

        TutorialUI.Instance.Hide();

        StartCoroutine(RestoreControlsNextFrame());
    }

    public void NextStep()
    {
        currentStepIndex++;

        if (currentStepIndex >= steps.Count)
        {
            EndTutorial();
            return;
        }

        ShowStep(steps[currentStepIndex]);
    }

    public void CompleteStep()
    {
        Debug.Log(
    $"Completing Step {currentStepIndex}"
);
        if (!isRunning)
            return;

        if (isTransitioningStep)
            return;

        isTransitioningStep = true;

        NextStep();
        Debug.Log(
            $"Moved to Step {currentStepIndex}"
        );
        isTransitioningStep = false;
    }

    void ShowStep(TutorialStep step)
    {
        if (step.pauseGame)
        {
            if (!tutorialPausedGame)
            {
                Pause();

                tutorialPausedGame = true;
            }
        }
        else
        {
            if (tutorialPausedGame)
            {
                Resume();

                tutorialPausedGame = false;
            }
        }

        bool shouldShowUI =
            !string.IsNullOrEmpty(
                step.description
            );

        if (shouldShowUI)
        {
            TutorialUI.Instance.Show(step);
        }
        else
        {
            TutorialUI.Instance.Hide();
        }
    }

    private IEnumerator RestoreControlsNextFrame()
    {
        yield return null;

        //UIManager.Instance.SwitchToPlayerControls();

        gameObject.SetActive(false);
        tutorialSetup.SetActive(false);
    }

    void CheckTrigger(
        TutorialTriggerType type
    )
    {
        if (!isRunning)
            return;

        if (currentStepIndex >= steps.Count)
            return;

        var step =
            steps[currentStepIndex];

        Debug.Log(
            $"Event: {type}, Current Step: {step.triggerType}"
        );

        if (step.triggerType == type)
        {
            CompleteStep();
        }
    }

    public static void Pause()
    {
        pauseCount++;

        Time.timeScale = 0f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopGameplayAudio();
        }

        Debug.Log(
            $"Game paused. Current pause count: {pauseCount}"
        );
    }

    public static void Resume()
    {
        pauseCount--;

        if (pauseCount <= 0)
        {
            pauseCount = 0;

            Time.timeScale = 1f;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ResumeAll();
            }

            Debug.Log(
                $"Game resumed. Current pause count: {pauseCount}"
            );
        }
    }
    private void HandlePlantScanned(CropField crop)
    {
        if (firstScannedPlant == null)
        {
            firstScannedPlant = crop;

            TutorialTargetRegistry.Instance.Register(
                "PlantFirstScanned",
                crop.transform);
        }

        CheckTrigger(TutorialTriggerType.OnPlantScanned);
    }

    private void HandlePlantTreated(CropField crop)
    {
        if (firstTreatedPlant == null)
        {
            firstTreatedPlant = crop;

            TutorialTargetRegistry.Instance.Register(
                "PlantFirstTreated",
                crop.transform);
        }

        CheckTrigger(TutorialTriggerType.OnPlantTreated);
    }

    public void ContinueTutorial()
    {
        if (!tutorialActive)
            return;

        TutorialStep step = CurrentStep;
        Debug.Log(
            $"ContinueTutorial called. Current Step: {currentStepIndex}, Trigger Type: {step?.triggerType}"
        );

        if (step == null)
            return;

        // Manual steps advance from the Continue button.
        if (step.triggerType == TutorialTriggerType.Manual)
        {
            CompleteStep();
        }
    }

    void HandlePlantScanned()
        => CheckTrigger(
            TutorialTriggerType.OnPlantScanned
        );

    void HandlePlantTreated()
        => CheckTrigger(
            TutorialTriggerType.OnPlantTreated
        );
    void HandleCameraModeChanged()
        => CheckTrigger(
            TutorialTriggerType.OnCameraModeChanged
        );

    //void HandleCookComplete()
    //    => CheckTrigger(
    //        TutorialTriggerType.OnCookComplete
    //    );

    //void HandleCustomerSpawned(
    //    Customer customer
    //)
    //{
    //    TutorialTargetRegistry.Instance
    //        .Register(
    //            "Customer",
    //            customer.transform
    //        );
    //}

    public bool IsTutorialRunning()
    {
        return tutorialActive;
    }

    [ContextMenu(
        "Reset Tutorial Save"
    )]
    public void ResetTutorial()
    {
        PlayerPrefs.DeleteKey(
            "TUTORIAL_COMPLETED"
        );

        PlayerPrefs.DeleteKey(
            "TUTORIAL_STARTED"
        );

        Debug.Log(
            "Tutorial Save Reset"
        );
    }
}