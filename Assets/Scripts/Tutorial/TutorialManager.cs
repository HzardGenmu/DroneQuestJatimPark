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
    private TutorialStep previousStep;

    private bool tutorialActive => isRunning && currentStepIndex >= 0 && currentStepIndex < steps.Count;

    public int CurrentStepIndex => currentStepIndex;
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

        if (previousStep != null)
        {
            foreach (GameObject obj in previousStep.activateWhileActive)
            {
                if (obj != null)
                    obj.SetActive(false);
            }

            previousStep = null;
        }

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
        if (!isRunning)
            return;

        if (isTransitioningStep)
            return;

        isTransitioningStep = true;

        currentStepIndex++;

        if (currentStepIndex >= steps.Count)
        {
            isTransitioningStep = false;
            EndTutorial();
            return;
        }

        ShowStep(steps[currentStepIndex]);

        isTransitioningStep = false;

        CheckCurrentStepState();

        Debug.Log(
            $"Moved to Step {currentStepIndex}"
        );
    }

    public void CompleteStep()
    {
        if (!isRunning)
            return;

        if (isTransitioningStep)
            return;

        Debug.Log(
            $"Completing Step {currentStepIndex}"
        );

        NextStep();
    }


    public void PreviousStep()
    {
        if (!isRunning)
            return;

        // Already at the first step.
        if (currentStepIndex <= 0)
        {
            Debug.Log("Already at the first tutorial step.");
            return;
        }

        if (isTransitioningStep)
            return;

        Debug.Log(
            $"Going back from Step {currentStepIndex} " +
            $"to Step {currentStepIndex - 1}"
        );

        isTransitioningStep = true;

        currentStepIndex--;

        ShowStep(steps[currentStepIndex]);

        isTransitioningStep = false;
    }

    void ShowStep(TutorialStep step)
    {
        if (previousStep != null)
        {
            foreach (GameObject obj in previousStep.activateWhileActive)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }

        // Enable current step objects
        foreach (GameObject obj in step.activateWhileActive)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        previousStep = step;

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

        GameEvents.OnTutorialStepStarted?.Invoke(step.triggerType);
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

    private void CheckCurrentStepState()
    {
        if (!tutorialActive)
            return;

        TutorialStep step = CurrentStep;

        if (step == null)
            return;

        switch (step.triggerType)
        {
            case TutorialTriggerType.OnCameraModeChanged:

                if (GameEvents.IsBottomCameraActive)
                {
                    Debug.Log(
                        "Camera already in bottom mode. Auto-completing camera tutorial step."
                    );

                    CompleteStep();
                }

                break;

            case TutorialTriggerType.OnPlantScanned:

                if (firstScannedPlant != null)
                {
                    CompleteStep();
                }

                break;

            case TutorialTriggerType.OnPlantTreated:

                if (firstTreatedPlant != null)
                {
                    CompleteStep();
                }

                break;
        }
    }

    //void HandlePlantScanned()
    //    => CheckTrigger(
    //        TutorialTriggerType.OnPlantScanned
    //    );

    //void HandlePlantTreated()
    //    => CheckTrigger(
    //        TutorialTriggerType.OnPlantTreated
    //    );
    private void HandleCameraModeChanged()
    {
        CheckTrigger(
            TutorialTriggerType.OnCameraModeChanged
        );
    }

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