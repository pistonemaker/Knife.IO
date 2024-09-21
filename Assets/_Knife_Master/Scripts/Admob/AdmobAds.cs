using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Sample;
using GoogleMobileAds.Ump.Api;
using UnityEngine.UI;

public class AdmobAds : MonoBehaviour
{
    public static AdmobAds Instance;
    public Transform adsContainer;
    public BannerViewController bannerController;
    public InterstitialAdController interstitialAdController;
    public AppOpenAdController appOpenAdController;
    public NativeOverlayAdController nativeOverlayAdController;
    public RewardedAdController rewardedAdController;
    public RewardedInterstitialAdController rewardedInterstitialAdController;

    public Image bannerImg;
    public Image interImg;
    public Image openAppImg;
    public Image nativeImg;
    public Image rewardImg;
    public Image rewardInterImg;

    private void Update()
    {
        if (bannerController.isLoaded)
        {
            bannerImg.color = Color.green;
        }
        else
        {
            bannerImg.color = Color.red;
        }
        
        if (interstitialAdController.isLoaded)
        {
            interImg.color = Color.green;
        }
        else
        {
            interImg.color = Color.red;
        }
        
        if (appOpenAdController.isLoaded)
        {
            openAppImg.color = Color.green;
        }
        else
        {
            openAppImg.color = Color.red;
        }
        
        if (nativeOverlayAdController.isLoaded)
        {
            nativeImg.color = Color.green;
        }
        else
        {
            nativeImg.color = Color.red;
        }
        
        if (rewardedAdController.isLoaded)
        {
            rewardImg.color = Color.green;
        }
        else
        {
            rewardImg.color = Color.red;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Always use test ads.
    // https://developers.google.com/admob/unity/test-ads
    internal static List<string> TestDeviceIds = new List<string>()
    {
        AdRequest.TestDeviceSimulator,
#if UNITY_IPHONE
            "96e23e80653bb28980d3f40beb58915c",
#elif UNITY_ANDROID
        "702815ACFC14FF222DA1DC767672A573"
#endif
    };

    // The Google Mobile Ads Unity plugin needs to be run only once.
    private static bool? _isInitialized;

    [SerializeField] private AdsConsentController _consentController;

    // Demonstrates how to configure Google Mobile Ads Unity plugin.
    private void Start()
    {
        // On Android, Unity is paused when displaying interstitial or rewarded video.
        // This setting makes iOS behave consistently with Android.
        MobileAds.SetiOSAppPauseOnBackground(true);

        // When true all events raised by GoogleMobileAds will be raised
        // on the Unity main thread. The default value is false.
        // https://developers.google.com/admob/unity/quick-start#raise_ad_events_on_the_unity_main_thread
        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        // Configure your RequestConfiguration with Child Directed Treatment
        // and the Test Device Ids.
        MobileAds.SetRequestConfiguration(new RequestConfiguration
        {
            TestDeviceIds = TestDeviceIds
        });

        // If we can request ads, we should initialize the Google Mobile Ads Unity plugin.
        if (_consentController.CanRequestAds)
        {
            InitializeGoogleMobileAds();
        }

        // Ensures that privacy and consent information is up to date.
        InitializeGoogleMobileAdsConsent();

        LoadStartGameAds();
        ShowBannerAds();
        ShowAppOpenAds();
    }

    private void LoadStartGameAds()
    {
        bannerController.LoadAd();
        appOpenAdController.LoadAd();
        nativeOverlayAdController.LoadAd();
    }

    // Ensures that privacy and consent information is up to date.
    private void InitializeGoogleMobileAdsConsent()
    {
        Debug.Log("Google Mobile Ads gathering consent.");

        _consentController.GatherConsent((string error) =>
        {
            if (error != null)
            {
                Debug.LogError("Failed to gather consent with error: " +
                               error);
            }
            else
            {
                Debug.Log("Google Mobile Ads consent updated: "
                          + ConsentInformation.ConsentStatus);
            }

            if (_consentController.CanRequestAds)
            {
                InitializeGoogleMobileAds();
            }
        });
    }

    // Initializes the Google Mobile Ads Unity plugin.
    private void InitializeGoogleMobileAds()
    {
        // The Google Mobile Ads Unity plugin needs to be run only once and before loading any ads.
        if (_isInitialized.HasValue)
        {
            return;
        }

        _isInitialized = false;

        // Initialize the Google Mobile Ads Unity plugin.
        Debug.Log("Google Mobile Ads Initializing.");
        MobileAds.Initialize((InitializationStatus initstatus) =>
        {
            if (initstatus == null)
            {
                Debug.LogError("Google Mobile Ads initialization failed.");
                _isInitialized = null;
                return;
            }

            // If you use mediation, you can check the status of each adapter.
            var adapterStatusMap = initstatus.getAdapterStatusMap();
            if (adapterStatusMap != null)
            {
                foreach (var item in adapterStatusMap)
                {
                    Debug.Log(string.Format("Adapter {0} is {1}",
                        item.Key,
                        item.Value.InitializationState));
                }
            }

            Debug.Log("Google Mobile Ads initialization complete.");
            _isInitialized = true;
        });
    }

    // Opens the AdInspector.
    public void OpenAdInspector()
    {
        Debug.Log("Opening ad Inspector.");
        MobileAds.OpenAdInspector((AdInspectorError error) =>
        {
            // If the operation failed, an error is returned.
            if (error != null)
            {
                Debug.Log("Ad Inspector failed to open with error: " + error);
                return;
            }

            Debug.Log("Ad Inspector opened successfully.");
        });
    }

    /// Opens the privacy options form for the user.
    /// Your app needs to allow the user to change their consent status at any time.
    public void OpenPrivacyOptions()
    {
        _consentController.ShowPrivacyOptionsForm((string error) =>
        {
            if (error != null)
            {
                Debug.LogError("Failed to show consent privacy form with error: " +
                               error);
            }
            else
            {
                Debug.Log("Privacy form opened successfully.");
            }
        });
    }

    public void ShowBannerAds(Action callBack = null)
    {
        if (bannerController.isLoaded)
        {
            bannerController.ShowAd();
            bannerController.OnAdsClose += callBack;
        }
        else
        {
            bannerController.LoadAd();
            bannerController.ShowAd();
            bannerController.OnAdsClose += callBack;
        }
    }

    public void ShowInterAds(Action callBack = null)
    {
        if (interstitialAdController.isLoaded)
        {
            StartCoroutine(ShowAdInter(callBack));
        }
        else
        {
            interstitialAdController.LoadAd();
            StartCoroutine(ShowAdInter(callBack));
        }
    }

    public void ShowAppOpenAds(Action callBack = null)
    {
        if (appOpenAdController.isLoaded)
        {
            StartCoroutine(ShowAdAppOpen(callBack));
        }
        else
        {
            appOpenAdController.LoadAd();
            StartCoroutine(ShowAdAppOpen(callBack));
        }
    }

    public void ShowNativeAds(Action callBack = null)
    {
        if (nativeOverlayAdController.isLoaded)
        {
            StartCoroutine(ShowAdNative(callBack));
        }
        else
        {
            callBack?.Invoke();
            nativeOverlayAdController.LoadAd();
            StartCoroutine(ShowAdNative(callBack));
        }
    }

    public void ShowRewardAds(Action callBack = null)
    {
        if (rewardedAdController.isLoaded)
        {
            StartCoroutine(ShowAdReward(callBack));
        }
        else
        {
            callBack?.Invoke();
            rewardedAdController.LoadAd();
            StartCoroutine(ShowAdReward(callBack));
        }
    }

    public void ShowRewardInterAds(Action callBack = null)
    {
        if (rewardedInterstitialAdController.isLoaded)
        {
            StartCoroutine(ShowAdRewardInter(callBack));
        }
        else
        {
            callBack?.Invoke();
            rewardedInterstitialAdController.LoadAd();
            StartCoroutine(ShowAdRewardInter(callBack));
        }
    }

    private IEnumerator ShowAdInter(Action callBack = null)
    {
        yield return new WaitForSeconds(0.5f);
        interstitialAdController.OnAdsClose += callBack;
        interstitialAdController.ShowAd();
    }

    private IEnumerator ShowAdAppOpen(Action callBack = null)
    {
        yield return new WaitForSeconds(0.5f);
        appOpenAdController.OnAdsClose += callBack;
        appOpenAdController.ShowAd();
    }

    private IEnumerator ShowAdNative(Action callBack = null)
    {
        yield return new WaitForSeconds(0.5f);
        nativeOverlayAdController.OnAdsClose += callBack;
        nativeOverlayAdController.ShowAd();
    }

    private IEnumerator ShowAdReward(Action callBack = null)
    {
        yield return new WaitForSeconds(0.5f);
        rewardedAdController.OnAdsClose += callBack;
        rewardedAdController.ShowAd();
    }

    private IEnumerator ShowAdRewardInter(Action callBack = null)
    {
        yield return new WaitForSeconds(0.5f);
        rewardedInterstitialAdController.OnAdsClose += callBack;
        rewardedInterstitialAdController.ShowAd();
    }
}