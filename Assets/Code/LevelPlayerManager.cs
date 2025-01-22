using UnityEngine;
using com.unity3d.mediation;
using System;
using GoogleMobileAds.Ump.Api;

public class LevelPlayerManager : MonoBehaviour
{
    public static LevelPlayerManager instance;
    public static string uniqueUserId = "demoUserUnity";

#if UNITY_ANDROID
    private string appKey = "85460dcd";
    private string bannerAdUnitId = "thnfvcsog13bhn08";
#elif UNITY_IPHONE
    private string appKey = "8545d445";
    private string bannerAdUnitId = "iep3rxsyp9na3rw8";
#else
    private string appKey = "unexpected_platform";
    private string bannerAdUnitId = "unexpected_platform";
#endif

    private LevelPlayBannerAd bannerAd;
    private int userTotalCredits = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Debug.Log("unity-script: Awake called");
    }

    private void Start()
    {
        // Create a ConsentRequestParameters object.
        ConsentRequestParameters request = new();

        // Check the current consent information status.
        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }

    void OnConsentInfoUpdated(FormError consentError)
    {
        if (consentError != null)
        {
            Debug.LogError(consentError);
            return;
        }

        ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
        {
            if (formError != null)
            {
                Debug.LogError(formError);
                return;
            }

            // Consent has been gathered, now update it in ironSource
            UpdateConsent();
        });
    }

    private void UpdateConsent()
    {
        string CMPString = PlayerPrefs.GetString("IABTCF_AddtlConsent");

        if (!string.IsNullOrEmpty(CMPString))
        {
            string[] CMPSlices = CMPString.Split("~");
            if (CMPSlices.Length >= 2)
            {
                string version = CMPSlices[0];
                if (version is "2" or "1")
                {
                    if (CMPSlices[1].Contains("2878"))
                    {
                        IronSource.Agent.setConsent(true);
                    }
                    else
                    {
                        IronSource.Agent.setConsent(false);
                    }
                }
            }
        }
        else
        {
            // If the consent string is not available or unrecognized, set to false or decide not to initialize
            IronSource.Agent.setConsent(false);
        }

        // Initialize ironSource only after processing consent
        InitializeIronSource();
    }

    private void InitializeIronSource()
    {
        IronSourceConfig.Instance.setClientSideCallbacks(true);
        IronSource.Agent.validateIntegration();
        Debug.Log("unity-script: unity version " + IronSource.unityVersion());

        LevelPlay.Init(appKey, uniqueUserId, new[] { LevelPlayAdFormat.REWARDED });

        LevelPlay.OnInitSuccess += OnInitializationCompleted;
        LevelPlay.OnInitFailed += (error => Debug.LogError("Initialization error: " + error));
    }

    private void OnInitializationCompleted(LevelPlayConfiguration configuration)
    {
        Debug.Log("Initialization completed.");
        LoadBanner();
        InitializeRewardedAds();
    }

    private void LoadBanner()
    {
        bannerAd = new LevelPlayBannerAd(bannerAdUnitId);

        bannerAd.OnAdLoaded += (adInfo) => Debug.Log("Banner Loaded: " + adInfo);
        bannerAd.OnAdLoadFailed += (error) => Debug.LogError("Banner Failed to Load: " + error);
        bannerAd.OnAdDisplayed += (adInfo) => Debug.Log("Banner Displayed: " + adInfo);
        bannerAd.LoadAd();
    }

    private void InitializeRewardedAds()
    {
        IronSourceRewardedVideoEvents.onAdOpenedEvent += (adInfo) => Debug.Log("Rewarded Video Opened: " + adInfo);
        IronSourceRewardedVideoEvents.onAdClosedEvent += (adInfo) => Debug.Log("Rewarded Video Closed: " + adInfo);
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
    }

    public void ShowRewardedVideo()
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            Debug.Log("Rewarded video not available");
        }
    }

    private void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        Debug.Log("Reward received for placement: " + placement);
        userTotalCredits += placement.getRewardAmount();
    }

    private void OnApplicationPause(bool pause) => IronSource.Agent.onApplicationPause(pause);

    private void OnDestroy() => bannerAd?.DestroyAd();
}
