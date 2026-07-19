using UnityEngine;
using UnityEngine.SceneManagement;

public class CongratulationsPanel : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private GameObject buttonContainer;

    [Header("Scenes")]
    [SerializeField] private string mapScene = "MapSelector";
    [SerializeField] private string menuScene = "MainMenu";

    private void Awake()
    {
        gameObject.SetActive(false);

        if (buttonContainer != null)
            buttonContainer.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        if (buttonContainer != null)
            buttonContainer.SetActive(true);
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