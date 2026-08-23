using UnityEngine;
using Unity.Services.LevelPlay;

public class AdsManager : MonoBehaviour
{
    [Header("LevelPlay App Keys")]
    [SerializeField] private string androidAppKey;
    [SerializeField] private string iosAppKey;

    [Header("LevelPlay Ad Unit IDs")]
    [SerializeField] private string bannerAndroid;
    [SerializeField] private string bannerIOS;

    private LevelPlayBannerAd bannerAd;

    private void Awake()
    {
        Debug.Log("[AdsManager] AWAKE");
    }

    private void OnEnable()
    {
        Debug.Log("[AdsManager] ON ENABLE");

        LevelPlay.OnInitSuccess += OnInitialized;
        LevelPlay.OnInitFailed += OnInitializationFailed;
    }

    private void OnDisable()
    {
        LevelPlay.OnInitSuccess -= OnInitialized;
        LevelPlay.OnInitFailed -= OnInitializationFailed;
    }

    private void Start()
    {
        Debug.Log("[AdsManager] START");

#if UNITY_ANDROID
        Debug.Log("[AdsManager] Initializing Android LevelPlay...");
        Debug.Log("[AdsManager] App Key: " + androidAppKey);

        LevelPlay.Init(androidAppKey);

#elif UNITY_IOS
        Debug.Log("[AdsManager] Initializing iOS LevelPlay...");
        LevelPlay.Init(iosAppKey);

#else
        Debug.Log("[AdsManager] Initializing Editor LevelPlay...");
        LevelPlay.Init("editor");
#endif
    }

    private void OnInitialized(LevelPlayConfiguration configuration)
    {
        Debug.Log("[AdsManager] LEVELPLAY INITIALIZED!");

#if UNITY_ANDROID
        string bannerAdUnitId = bannerAndroid;
#elif UNITY_IOS
        string bannerAdUnitId = bannerIOS;
#else
        string bannerAdUnitId = "editor_banner";
#endif

        Debug.Log("[AdsManager] Banner ID: " + bannerAdUnitId);

        var config = new LevelPlayBannerAd.Config.Builder()
            .SetSize(LevelPlayAdSize.BANNER)
            .SetPosition(LevelPlayBannerPosition.TopCenter)
            .SetDisplayOnLoad(true)
            .SetRespectSafeArea(true)
            .Build();

        Debug.Log("[AdsManager] Creating banner...");

        bannerAd = new LevelPlayBannerAd(
            bannerAdUnitId,
            config
        );

        bannerAd.OnAdLoaded += OnBannerLoaded;
        bannerAd.OnAdLoadFailed += OnBannerLoadFailed;
        bannerAd.OnAdDisplayed += OnBannerDisplayed;
        bannerAd.OnAdDisplayFailed += OnBannerDisplayFailed;
        bannerAd.OnAdClicked += OnBannerClicked;

        Debug.Log("[AdsManager] Loading banner...");

        bannerAd.LoadAd();
    }

    private void OnInitializationFailed(LevelPlayInitError error)
    {
        Debug.LogError(
            "[AdsManager] LEVELPLAY INITIALIZATION FAILED: " + error
        );
    }

    private void OnBannerLoaded(LevelPlayAdInfo adInfo)
    {
        Debug.Log("[AdsManager] BANNER LOADED!");
    }

    private void OnBannerLoadFailed(LevelPlayAdError error)
    {
        Debug.LogError(
            "[AdsManager] BANNER LOAD FAILED: " + error
        );
    }

    private void OnBannerDisplayed(LevelPlayAdInfo adInfo)
    {
        Debug.Log("[AdsManager] BANNER DISPLAYED!");
    }

    private void OnBannerDisplayFailed(
        LevelPlayAdInfo adInfo,
        LevelPlayAdError error)
    {
        Debug.LogError(
            "[AdsManager] BANNER DISPLAY FAILED: " + error
        );
    }

    private void OnBannerClicked(LevelPlayAdInfo adInfo)
    {
        Debug.Log("[AdsManager] BANNER CLICKED!");
    }
}