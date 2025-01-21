//using GoogleMobileAds.Ump.Api;
using UnityEngine;
using UnityEngine.Events;

public class LevelPlayerManager : MonoBehaviour
{
    public static LevelPlayerManager instance;

    [SerializeField] private string appKey = "20c4450e5";

    UnityAction<bool, int> rewardedCallBack;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /*private void Start()
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
    }*/

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
        IronSource.Agent.setMetaData("AdMob_TFCD", "false");
        IronSource.Agent.setMetaData("AdMob_TFUA", "false");
        IronSource.Agent.setMetaData("AdMob_MaxContentRating", "MAX_AD_CONTENT_RATING_MA");
        IronSource.Agent.setMetaData("is_test_suite", "enable");

        IronSource.Agent.validateIntegration();
        IronSource.Agent.init(appKey);

        IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;

        IronSourceBannerEvents.onAdLoadedEvent += BannerOnAdLoadedEvent;
        IronSourceBannerEvents.onAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
        IronSourceBannerEvents.onAdClickedEvent += BannerOnAdClickedEvent;
        IronSourceBannerEvents.onAdScreenPresentedEvent += BannerOnAdScreenPresentedEvent;
        IronSourceBannerEvents.onAdScreenDismissedEvent += BannerOnAdScreenDismissedEvent;
        IronSourceBannerEvents.onAdLeftApplicationEvent += BannerOnAdLeftApplicationEvent;

        IronSourceEvents.onSdkInitializationCompletedEvent += SDKInitialized;
    }

    private void SDKInitialized()
    {
        Debug.Log("SDK has initialized.");
        IronSource.Agent.launchTestSuite();
        LoadBanner();
    }

    private void OnApplicationPause(bool pause) => IronSource.Agent.onApplicationPause(pause);

    public void ShowReward(UnityAction<bool, int> rewardedCallback)
    {
        rewardedCallBack = rewardedCallback;

        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            Debug.Log("Reward video unavailable.");
            rewardedCallback(false, 0);
        }
    }

    /************* RewardedVideo AdInfo Delegates *************/
    // Indicates that there’s an available ad.
    // The adInfo object includes information about the ad that was loaded successfully
    // This replaces the RewardedVideoAvailabilityChangedEvent(true) event
    void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo) => Debug.Log("Reward video available.");

    // Indicates that no ads are available to be displayed
    // This replaces the RewardedVideoAvailabilityChangedEvent(false) event
    void RewardedVideoOnAdUnavailable()
    {
        Debug.Log("Reward video unavailable.");
        rewardedCallBack(false, 0);
    }

    // The Rewarded Video ad view has opened. Your activity will loose focus.
    void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo) => Debug.Log("Ad opened.");

    // The Rewarded Video ad view is about to be closed. Your activity will regain its focus.
    void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo) => Debug.Log("Ad closed.");

    // The user completed to watch the video, and should be rewarded.
    // The placement parameter will include the reward data.
    // When using server-to-server callbacks, you may ignore this event and wait for the ironSource server callback.
    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        Debug.Log($"Reward has been rewarded: {placement.getRewardAmount()}.");
        rewardedCallBack(true, placement.getRewardAmount());
    }

    // The rewarded video ad was failed to show.
    void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
    {
        Debug.Log("Ad failed to show.");
        rewardedCallBack(false, 0);
    }

    // Invoked when the video ad was clicked.
    // This callback is not supported by all networks, and we recommend using it only if
    // it’s supported by all networks you included in your build.
    void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo) => Debug.Log("Ad clicked.");

    private void LoadBanner() => IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.TOP);

    private void DestroyBanner() => IronSource.Agent.destroyBanner();

    /************* Banner AdInfo Delegates *************/

    //Invoked once the banner has loaded
    void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo) => Debug.Log("Banner loaded.");

    //Invoked when the banner loading process has failed.
    void BannerOnAdLoadFailedEvent(IronSourceError ironSourceError) => Debug.Log($"Banner failed to load: {ironSourceError}.");

    // Invoked when end user clicks on the banner ad
    void BannerOnAdClickedEvent(IronSourceAdInfo adInfo) => Debug.Log("Banner clicked.");

    //Notifies the presentation of a full screen content following user click
    void BannerOnAdScreenPresentedEvent(IronSourceAdInfo adInfo) => Debug.Log("Banner screen presented.");

    //Notifies the presented screen has been dismissed
    void BannerOnAdScreenDismissedEvent(IronSourceAdInfo adInfo) => Debug.Log("Banner screen dismissed");

    //Invoked when the user leaves the app
    void BannerOnAdLeftApplicationEvent(IronSourceAdInfo adInfo) => Debug.Log("Banner left application");
}
