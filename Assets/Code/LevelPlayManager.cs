using com.unity3d.mediation;
using GoogleMobileAds.Ump.Api;
using System.Collections;
using UnityEngine;

public class LevelPlayManager : MonoBehaviour
{
    internal static LevelPlayManager instance;

    private LevelPlayBannerAd bannerAd;
    private LevelPlayRewardedAd rewardedAd;

    private bool adReady = false;

#if UNITY_ANDROID
    private string appKey = "20c4450e5";
    private string bannerAdUnitId = "myu5mmnobtfji56s";
    private string videoaAdUnitId = "axnq5x66xc9jp0wn";
#else
    private string appKey = "unexpected_platform";
    private string bannerAdUnitId = "unexpected_platform";
#endif

    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        //ConsentInformation.Reset();

        // Create a ConsentRequestParameters object.
        ConsentRequestParameters request = new();

        // Check the current consent information status.
        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }

    private void OnConsentInfoUpdated(FormError consentError)
    {
        if (consentError != null)
        {
            Debug.LogError("Consent info update failed: " + consentError);
            return;
        }

        // Check if consent is already given or not required
        if (ConsentInformation.ConsentStatus is not ConsentStatus.Obtained and not ConsentStatus.NotRequired)
        {
            PlayerManager.DisableInput();
            Debug.Log("Loading form");

            // Consent not obtained, show the consent form
            ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
            {
                if (formError != null)
                {
                    Debug.LogError("Consent form error: " + formError);
                    return;
                }

                PlayerManager.EnableInput();
                InitializeIronSource();
            });
        }
        else
        {
            Debug.Log($"Consent status: {ConsentInformation.ConsentStatus}");
            InitializeIronSource();
        }
    }

    private void InitializeIronSource()
    {
        Debug.Log("Intitializing IronSource");

        IronSource.Agent.shouldTrackNetworkState(true);
        IronSource.Agent.setMetaData("do_not_sell", "true");
        IronSource.Agent.setMetaData("is_child_directed", "false");
        IronSource.Agent.setMetaData("AdMob_TFCD", "false");
        IronSource.Agent.setMetaData("AdMob_TFUA", "false");
        IronSource.Agent.setMetaData("AdMob_MaxContentRating", "MAX_AD_CONTENT_RATING_MA");
        IronSource.Agent.setMetaData("UnityAds_coppa", "false");
        //IronSource.Agent.validateIntegration();
        //IronSource.Agent.setMetaData("is_test_suite", "enable");

        LevelPlay.OnInitSuccess += OnInitializationCompleted;
        LevelPlay.OnInitFailed += error => Debug.LogError("Initialization error: " + error.ErrorCode + "\n" + error.ErrorMessage);

        LevelPlay.Init(appKey, adFormats: new[] { LevelPlayAdFormat.REWARDED });
    }

    private void OnInitializationCompleted(LevelPlayConfiguration configuration)
    {
        Debug.Log("Initialization completed.");
        //IronSource.Agent.launchTestSuite();
        InitializeBannerAds();
        InitializeRewardedAds();
    }

    private void InitializeBannerAds()
    {
        Debug.Log("Initializing Banner Ads");

        bannerAd = new LevelPlayBannerAd(bannerAdUnitId, LevelPlayAdSize.CreateAdaptiveAdSize(), LevelPlayBannerPosition.TopCenter, "Startup", respectSafeArea: true);

        bannerAd.OnAdLoaded += (adInfo) => Debug.Log($"Banner loaded: {adInfo}");
        bannerAd.OnAdLoadFailed += (adInfo) => Debug.Log($"Banner failed to load: {adInfo}");
        bannerAd.OnAdDisplayed += (adInfo) => Debug.Log($"Banner displayed: {adInfo}");

        bannerAd.LoadAd();

        Debug.Log("Banner Ads Initialized");
    }

    private void ShowBanner() => bannerAd.ShowAd();

    private void HideBanner() => bannerAd.HideAd();

    private void InitializeRewardedAds()
    {
        Debug.Log("Initializing Rewarded Ads");

        rewardedAd = new LevelPlayRewardedAd(videoaAdUnitId);

        rewardedAd.OnAdLoaded += (adInfo) => Debug.Log($"Video ad loaded: {adInfo}");
        rewardedAd.OnAdLoadFailed += (adInfo) => Debug.Log($"Video ad failed to load: {adInfo}");
        rewardedAd.OnAdDisplayed += RewardedAdDisplayed;
        rewardedAd.OnAdDisplayFailed += (adInfo) => Debug.Log($"Video ad failed to display: {adInfo}");
        rewardedAd.OnAdRewarded += (adInfo, reward) => Debug.Log($"Video ad reward: {reward}");
        rewardedAd.OnAdClosed += RewardedAdClosed;

        StartCoroutine(LoadRewardedAd());

        Debug.Log("Rewarded Ads Initialized");
    }

    private IEnumerator LoadRewardedAd()
    {
        adReady = false;

        while (!rewardedAd.IsAdReady())
        {
            rewardedAd.LoadAd();
            yield return new WaitForSeconds(5f);
        }

        adReady = true;
    }

    internal bool IsAdReady() => adReady;

    internal void ShowRewardedAd()
    {
        Debug.Log("Checking for ready ad...");

        if (IsAdReady())
        {
            Debug.Log("Showing video ad");
            rewardedAd.ShowAd("Turn_Complete");
        }
        else
        {
            Debug.Log("Video ad not available");
        }
    }

    void RewardedAdDisplayed(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"Video ad displayed: {adInfo}");
        HideBanner();
    }

    void RewardedAdClosed(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"Video ad closed: {adInfo}");
        StartCoroutine(GameSettings.instance.CoverScreen());

        ShowBanner();
        StartCoroutine(LoadRewardedAd());

        GameCoordinator.instance.SecondChance();
    }

    private void OnApplicationPause(bool pause) => IronSource.Agent.onApplicationPause(pause);

    private void OnDestroy()
    {
        rewardedAd.OnAdDisplayed -= RewardedAdDisplayed;
        rewardedAd.OnAdClosed -= RewardedAdClosed;
        bannerAd?.DestroyAd();
    }
}
