using UnityEngine;
using GoogleMobileAds.Api;

public class GoogleBannerTest : MonoBehaviour
{
    private const string AndroidBannerAdUnitId =
        "ca-app-pub-3940256099942544/6300978111";

    private BannerView bannerView;

    private void Start()
    {
        Debug.Log("[GoogleBannerTest] Starting Google Mobile Ads...");

        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("[GoogleBannerTest] Google Mobile Ads initialized.");

            LoadBanner();
        });
    }

    private void LoadBanner()
    {
        Debug.Log("[GoogleBannerTest] Creating banner...");

        bannerView = new BannerView(
            AndroidBannerAdUnitId,
            AdSize.Banner,
            AdPosition.Top
        );

        bannerView.OnBannerAdLoaded += OnBannerLoaded;
        bannerView.OnBannerAdLoadFailed += OnBannerLoadFailed;

        Debug.Log("[GoogleBannerTest] Loading test banner...");

        bannerView.LoadAd(new AdRequest());
    }

    private void OnBannerLoaded()
    {
        Debug.Log("[GoogleBannerTest] TEST BANNER LOADED!");
    }

    private void OnBannerLoadFailed(LoadAdError error)
    {
        Debug.LogError(
            "[GoogleBannerTest] TEST BANNER FAILED: " + error
        );
    }

    private void OnDestroy()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }
    }
}