using com.unity3d.mediation;
using GoogleMobileAds.Ump.Api;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelPlayerManager : MonoBehaviour
{
    public static LevelPlayerManager instance;

    [SerializeField] private Button continueButton;

#if UNITY_ANDROID
    private string appKey = "20c4450e5";
    private string bannerAdUnitId = "myu5mmnobtfji56s";
#elif UNITY_IPHONE
    private string appKey = "8545d445";
    private string bannerAdUnitId = "iep3rxsyp9na3rw8";
#else
    private string appKey = "unexpected_platform";
    private string bannerAdUnitId = "unexpected_platform";
#endif

    private LevelPlayBannerAd bannerAd;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoad;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        ConsentInformation.Reset();
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

            InitializeIronSource();
        });
    }

    /*
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
    */

    private void InitializeIronSource()
    {
        IronSource.Agent.validateIntegration();
        IronSource.Agent.shouldTrackNetworkState(true);
        IronSource.Agent.setMetaData("AdMob_TFCD", "false");
        IronSource.Agent.setMetaData("AdMob_TFUA", "false");
        IronSource.Agent.setMetaData("AdMob_MaxContentRating", "MAX_AD_CONTENT_RATING_MA");
        IronSource.Agent.setMetaData("is_test_suite", "enable");

        LevelPlay.Init(appKey, adFormats: new[] { LevelPlayAdFormat.REWARDED });

        LevelPlay.OnInitSuccess += OnInitializationCompleted;
        LevelPlay.OnInitFailed += error => Debug.LogError("Initialization error: " + error);
    }

    private void OnInitializationCompleted(LevelPlayConfiguration configuration)
    {
        Debug.Log("Initialization completed.");
        IronSource.Agent.launchTestSuite();
        LoadBanner();
        InitializeRewardedAds();
    }

    private void LoadBanner()
    {
        bannerAd = new LevelPlayBannerAd(bannerAdUnitId, LevelPlayAdSize.CreateAdaptiveAdSize(), LevelPlayBannerPosition.TopCenter, "Startup", respectSafeArea: true);

        bannerAd.OnAdLoaded += (adInfo) => Debug.Log("Banner Loaded: " + adInfo);
        bannerAd.OnAdLoadFailed += BannerOnAdLoadFailedEvent;
        bannerAd.OnAdDisplayed += (adInfo) => Debug.Log("Banner Displayed: " + adInfo);
        bannerAd.LoadAd();
    }

    private void ShowBanner() => bannerAd.ShowAd();

    private void HideBanner() => bannerAd.HideAd();

    private void BannerOnAdLoadFailedEvent(LevelPlayAdError error)
    {
        Debug.LogError("Banner Failed to Load: " + error + "\nRetyring in 5 seconds");
        //StartCoroutine(ReloadBanner());
    }

    private IEnumerator ReloadBanner()
    {
        yield return new WaitForSeconds(5f);
        Debug.Log("Reloading banner...");
        bannerAd.LoadAd();
    }

    private void InitializeRewardedAds()
    {
        IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
    }

    public void ShowRewardedVideo()
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo("Turn_Complete");
        }
        else
        {
            Debug.Log("Rewarded video not available");
        }
    }

    void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Rewarded Video Opened: " + adInfo);
        HideBanner();
    }
    void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Rewarded Video Closed: " + adInfo);
        ShowBanner();
    }

    private void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        Debug.Log("Reward received for placement: " + placement);

    }

    private void OnApplicationPause(bool pause) => IronSource.Agent.onApplicationPause(pause);

    private void OnDestroy() => bannerAd?.DestroyAd();

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().buildIndex is 1)
        {
            continueButton = GameObject.Find("Continue").GetComponent<Button>();
            continueButton.onClick.AddListener(() => ShowRewardedVideo());
        }
    }
}
