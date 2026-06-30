using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapLevelButton : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] private int levelIndex = 1;
    [SerializeField] private LevelData levelData;

    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private RectTransform root;

    [SerializeField] private GameObject lockObject;
    [SerializeField] private GameObject numberObject;

    [SerializeField] private TMP_Text levelNumberText;

    [SerializeField] private Image[] stars;

    [Header("Sprites")]
    [SerializeField] private Sprite filledStar;
    [SerializeField] private Sprite emptyStar;

    [Header("Animation")]
    [SerializeField] private float selectedScale = 1.15f;
    [SerializeField] private float animationDuration = 0.25f;

    private bool unlocked;
    private bool selected;

    public LevelData Data => levelData;
    public int LevelIndex => levelIndex;
    public bool IsUnlocked => unlocked;

    private void Awake()
    {
        button.onClick.AddListener(OnClicked);

        levelNumberText.text = levelIndex.ToString();
    }

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        unlocked =
            MapSaveSystem.Instance.IsUnlocked(levelIndex);

        numberObject.SetActive(unlocked);
        lockObject.SetActive(!unlocked);

        button.interactable = unlocked;

        int earnedStars =
            MapSaveSystem.Instance.GetStars(levelIndex);

        UpdateStars(earnedStars);

        root.localScale = Vector3.one;
    }

    private void UpdateStars(int amount)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].sprite =
                i < amount ?
                filledStar :
                emptyStar;
        }
    }

    private void OnClicked()
    {
        if (!unlocked)
            return;

        MapSelector.Instance.SelectLevel(this);
    }

    public void Select()
    {
        selected = true;

        root.DOKill();

        root
            .DOScale(selectedScale, animationDuration)
            .SetEase(Ease.OutBack);

        root
            .DOPunchRotation(
                new Vector3(0, 0, 6),
                0.25f,
                8,
                0.8f);
    }

    public void Deselect()
    {
        selected = false;

        root.DOKill();

        root
            .DOScale(1f, animationDuration)
            .SetEase(Ease.OutBack);

        root.rotation = Quaternion.identity;
    }

    public void PlayUnlockAnimation()
    {
        unlocked = true;

        lockObject.SetActive(false);
        numberObject.SetActive(true);

        button.interactable = true;

        root.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();

        seq.Append(
            root.DOScale(1.25f, 0.25f)
                .SetEase(Ease.OutBack));

        seq.Append(
            root.DOScale(1f, 0.15f));

        seq.Join(
            root.DOPunchRotation(
                new Vector3(0, 0, 10),
                0.35f,
                10,
                0.8f));
    }
}