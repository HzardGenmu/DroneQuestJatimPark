using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelPopup : MonoBehaviour
{
    [SerializeField] private LevelSelectManager manager;

    [Header("UI")]
    [SerializeField] private GameObject panel;

    //[SerializeField] private TMP_Text levelNameText;
    //[SerializeField] private TMP_Text descriptionText;
    //[SerializeField] private TMP_Text cropText;
    //[SerializeField] private TMP_Text locationText;
    //[SerializeField] private TMP_Text objectiveText;
    //[SerializeField] private TMP_Text difficultyText;

    //[SerializeField] private Image previewImage;

    private LevelData currentLevel;

    public void OpenPopup()
    {
        currentLevel = manager.CurrentLevel;

        //levelNameText.text = currentLevel.levelName;
        //descriptionText.text = currentLevel.description;
        //cropText.text = currentLevel.cropType;
        //locationText.text = currentLevel.location;
        //objectiveText.text = currentLevel.objective;
        //difficultyText.text = currentLevel.difficulty.ToString();

        //previewImage.sprite = currentLevel.previewImage;

        panel.SetActive(true);
    }

    public void ClosePopup()
    {
        panel.SetActive(false);
    }
}