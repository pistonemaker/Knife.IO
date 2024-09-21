using System;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Sample
{
    public class AppOpenAdController : MonoBehaviour
    {
#if UNITY_ANDROID
        //private const string _adUnitId = "ca-app-pub-3940256099942544/9257395921";
        private const string _adUnitId = "ca-app-pub-4608150884660102/6369321744";
#elif UNITY_IPHONE
        private const string _adUnitId = "ca-app-pub-3940256099942544/5575463023";
#else
        private const string _adUnitId = "unused";
#endif

        // App open ads can be preloaded for up to 4 hours.
        private readonly TimeSpan TIMEOUT = TimeSpan.FromHours(4);
        private DateTime _expireTime;
        private AppOpenAd _appOpenAd;
        public Action OnAdsClose;
        public bool isLoaded = false;
        public bool isSucess = false;

        private void Awake()
        {
            // Use the AppStateEventNotifier to listen to application open/close events.
            // This is used to launch the loaded ad when we open the app.
            AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
        }

        private void OnDestroy()
        {
            // Always unlisten to events when complete.
            AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
        }

        // Loads the ad.
        public void LoadAd()
        {
            // Clean up the old ad before loading a new one.
            if (_appOpenAd != null)
            {
                DestroyAd();
            }

            Debug.Log("Loading app open ad.");

            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // Send the request to load the ad.
            AppOpenAd.Load(_adUnitId, adRequest, (AppOpenAd ad, LoadAdError error) =>
                {
                    // If the operation failed with a reason.
                    if (error != null)
                    {
                        isLoaded = false;
                        Debug.LogError("App open ad failed to load an ad with error : "
                                        + error);
                        return;
                    }

                    // If the operation failed for unknown reasons.
                    // This is an unexpected error, please report this bug if it happens.
                    if (ad == null)
                    {
                        isLoaded = false;
                        Debug.LogError("Unexpected error: App open ad load event fired with " +
                                       " null ad and null error.");
                        return;
                    }

                    // The operation completed successfully.
                    Debug.Log("App open ad loaded with response : " + ad.GetResponseInfo());
                    _appOpenAd = ad;
                    isLoaded = true;

                    // App open ads can be preloaded for up to 4 hours.
                    _expireTime = DateTime.Now + TIMEOUT;

                    // Register to ad events to extend functionality.
                    RegisterEventHandlers(ad);
                });
        }

        // Shows the ad.
        public void ShowAd()
        {
            // App open ads can be preloaded for up to 4 hours.
            if (_appOpenAd != null && _appOpenAd.CanShowAd() && DateTime.Now < _expireTime)
            {
                isSucess = true;
                OpenAppManager.Instance.canShowOpenAppAds = false;
                Debug.Log("Showing app open ad.");
                _appOpenAd.Show();
            }
            else
            {
                isSucess = false;
                Debug.LogError("App open ad is not ready yet.");
            }
        }

        // Destroys the ad.
        public void DestroyAd()
        {
            if (_appOpenAd != null)
            {
                isLoaded = false;
                isSucess = false;
                Debug.Log("Destroying app open ad.");
                _appOpenAd.Destroy();
                _appOpenAd = null;
            }
        }

        // Logs the ResponseInfo.
        public void LogResponseInfo()
        {
            if (_appOpenAd != null)
            {
                var responseInfo = _appOpenAd.GetResponseInfo();
                Debug.Log(responseInfo);
            }
        }

        private void OnAppStateChanged(AppState state)
        {
            Debug.Log("App State changed to : " + state);

            // If the app is Foregrounded and the ad is available, show it.
            if (state == AppState.Foreground)
            {
                ShowAd();
            }
        }

        private void RegisterEventHandlers(AppOpenAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("App open ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("App open ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("App open ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("App open ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("App open ad full screen content closed.");

                // It may be useful to load a new ad when the current one is complete.
                LoadAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("App open ad failed to open full screen content with error : "
                                + error);
            };
        }
    }
}
