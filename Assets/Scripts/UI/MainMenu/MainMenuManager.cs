using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private DroneDispatchAnimator dispatchAnimator;

    public void PlayGame()
    {
        dispatchAnimator.BeginDispatch();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}