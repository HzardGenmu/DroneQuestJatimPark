using UnityEngine;
using System.Collections;

public class GameplayAudio : MonoBehaviour
{
    private AudioLibrary library;

    private AudioHandle sprayHandle;

    private AudioHandle droneHandle;

    private void Start()
    {
        library = AudioManager.Instance.audioLibrary;
    }

    private void OnEnable()
    {
        GameEvents.OnPlantCompleted += PlayPlantComplete;

        GameEvents.OnSprayStarted += StartSpray;
        GameEvents.OnSprayStopped += StopSpray;

        GameEvents.OnDroneLanding += DroneLanding;
        GameEvents.OnDroneTakeoff += DroneTakeoff;

        GameEvents.OnBatteryDepleted += StopDroneAudio;
    }

    private void OnDisable()
    {
        GameEvents.OnPlantCompleted -= PlayPlantComplete;

        GameEvents.OnSprayStarted -= StartSpray;
        GameEvents.OnSprayStopped -= StopSpray;

        GameEvents.OnDroneLanding -= DroneLanding;
        GameEvents.OnDroneTakeoff -= DroneTakeoff;

        GameEvents.OnBatteryDepleted += StopDroneAudio;
    }

    private void PlayPlantComplete()
    {
        AudioManager.Instance.Play(library.plantDone);
    }

    private void StartSpray()
    {
        if (sprayHandle != null &&
            sprayHandle.IsPlaying)
            return;

        sprayHandle =
            AudioManager.Instance.Play(
                library.spray);
    }

    private void StopSpray()
    {
        if (sprayHandle == null)
            return;

        AudioManager.Instance.Stop(sprayHandle);
        sprayHandle = null;
    }

    private void DroneLanding()
    {
        AudioManager.Instance.Pause(droneHandle);

        AudioManager.Instance.Play(library.droneLand);
    }

    private void DroneTakeoff()
    {
        StartCoroutine(DroneTakeoffRoutine());
    }

    private IEnumerator DroneTakeoffRoutine()
    {
        AudioManager.Instance.Play(library.droneLift);

        yield return new WaitForSeconds(
            library.droneLift.endTime -
            library.droneLift.startTime);

        if (droneHandle == null || !droneHandle.IsValid)
        {
            droneHandle =
                AudioManager.Instance.Play(library.droneFly);
        }
        else
        {
            AudioManager.Instance.Resume(droneHandle);
        }
    }

    private void StopDroneAudio()
    {
        StopAllCoroutines();

        if (sprayHandle != null)
        {
            AudioManager.Instance.Stop(sprayHandle);
            sprayHandle = null;
        }

        if (droneHandle != null)
        {
            AudioManager.Instance.Stop(droneHandle);
            droneHandle = null;
        }
    }
}