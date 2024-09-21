using System;
using UnityEngine;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Sample
{
    public class InterstitialAdController : MonoBehaviour
    {
#if UNITY_ANDROID
        //private const string _adUnitId = "ca-app-pub-3940256099942544/1033173712";
        private const string _adUnitId = "ca-app-pub-4608150884660102/7712724396";
#elif UNITY_IPHONE
        private const string _adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        private const string _adUnitId = "unused";
#endif

        private InterstitialAd _interstitialAd;
        public Action OnAdsClose;
        public bool isLoaded = false;
        public bool isSucess = false;

        // Loads the ad.
        public void LoadAd()
        {
            // Clean up the old ad before loading a new one.
            if (_interstitialAd != null)
            {
                DestroyAd();
            }

            Debug.Log("Loading interstitial ad.");

            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // Send the request to load the ad.
            InterstitialAd.Load(_adUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
            {
                // If the operation failed with a reason.
                if (error != null)
                {
                    isLoaded = false;
                    Debug.LogError("Interstitial ad failed to load an ad with error : " + error);
                    return;
                }

                // If the operation failed for unknown reasons.
                // This is an unexpected error, please report this bug if it happens.
                if (ad == null)
                {
                    isLoaded = false;
                    Debug.LogError("Unexpected error: Interstitial load event fired with null ad and null error.");
                    return;
                }

                // The operation completed successfully.
                Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());
                _interstitialAd = ad;
                isLoaded = true;

                // Register to ad events to extend functionality.
                RegisterEventHandlers(ad);
            });
        }

        // Shows the ad.
        public void ShowAd()
        {
            if (_interstitialAd != null && _interstitialAd.CanShowAd())
            {
                isSucess = true;
                Debug.Log("Showing interstitial ad.");
                _interstitialAd.Show();
            }
            else
            {
                isSucess = false;
                Debug.LogError("Interstitial ad is not ready yet.");
            }
            
            SetInterDontDestroyOnLoad();
        }

        public void SetInterDontDestroyOnLoad()
        {
            GameObject inter = GameObject.Find("768x1024(Clone)");
            if (inter == null)
            {
                Debug.LogError("Inter is null");
            }

            inter.transform.SetParent(AdmobAds.Instance.adsContainer);
        }

        // Destroys the ad.
        public void DestroyAd()
        {
            Debug.Log("!!!Destroy Ad");
            if (_interstitialAd != null)
            {
                isLoaded = false;
                isSucess = false;
                Debug.Log("Destroying interstitial ad.");
                _interstitialAd.Destroy();
                _interstitialAd = null;
            }
        }

        // Logs the ResponseInfo.
        public void LogResponseInfo()
        {
            if (_interstitialAd != null)
            {
                var responseInfo = _interstitialAd.GetResponseInfo();
                Debug.Log(responseInfo);
            }
        }

        private void RegisterEventHandlers(InterstitialAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () => { Debug.Log("Interstitial ad recorded an impression."); };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () => { Debug.Log("Interstitial ad was clicked."); };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Interstitial ad full screen content opened.");
                OnAdsClose?.Invoke();
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial ad full screen content closed.");
                // Load an new ad
                LoadAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Interstitial ad failed to open full screen content with error : "
                               + error);
            };
        }
    }
}