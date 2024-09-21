using System;
using UnityEngine;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Sample
{
    public class BannerViewController : MonoBehaviour
    {
#if UNITY_ANDROID
        //private const string _adUnitId = "ca-app-pub-3940256099942544/6300978111";
        private const string _adUnitId = "ca-app-pub-4608150884660102/8728957432";
#elif UNITY_IPHONE
        private const string _adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        private const string _adUnitId = "unused";
#endif

        private BannerView _bannerView;
        public Action OnAdsClose;
        
        public bool isLoaded = false;
        public bool isSucess = false;

        // Creates a 320x50 banner at top of the screen.
        public void CreateBannerView()
        {
            Debug.Log("Creating banner view.");

            // If we already have a banner, destroy the old one.
            if(_bannerView != null)
            {
                DestroyAd();
            }

            // Create a 320x50 banner at top of the screen.
            _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Bottom);

            // Listen to events the banner may raise.
            ListenToAdEvents();

            Debug.Log("Banner view created.");
        }

        // Creates the banner view and loads a banner ad.
        public void LoadAd()
        {
            // Create an instance of a banner view first.
            if(_bannerView == null)
            {
                isLoaded = false;
                CreateBannerView();
            }

            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // Send the request to load the ad.
            Debug.Log("Loading banner ad.");
            
            _bannerView.LoadAd(adRequest);
            isLoaded = true;
            SetBannerDontDestroyOnLoad();
        }

        public void SetBannerDontDestroyOnLoad()
        {
            GameObject banner = GameObject.Find("BANNER(Clone)");
            if (banner == null)
            {
                Debug.LogError("Banner is null");
                return;
            }

            banner.transform.SetParent(AdmobAds.Instance.adsContainer);
        }

        // Shows the ad.
        public void ShowAd()
        {
            if (_bannerView != null)
            {
                isSucess = true; 
                Debug.Log("Showing banner view.");
                _bannerView.Show();
            }
        }

        // Hides the ad.
        public void HideAd()
        {
            if (_bannerView != null)
            {
                Debug.Log("Hiding banner view.");
                _bannerView.Hide();
            }
        }

        /// Destroys the ad.
        /// When you are finished with a BannerView, make sure to call
        /// the Destroy() method before dropping your reference to it.
        public void DestroyAd()
        {
            if (_bannerView != null)
            {
                isLoaded = false;
                isSucess = false;
                Debug.Log("Destroying banner view.");
                _bannerView.Destroy();
                _bannerView = null;
            }
        }

        // Logs the ResponseInfo.
        public void LogResponseInfo()
        {
            if (_bannerView != null)
            {
                var responseInfo = _bannerView.GetResponseInfo();
                if (responseInfo != null)
                {
                    Debug.Log(responseInfo);
                }
            }
        }

        // Listen to events the banner may raise.
        private void ListenToAdEvents()
        {
            // Raised when an ad is loaded into the banner view.
            _bannerView.OnBannerAdLoaded += () =>
            {
                Debug.Log("Banner view loaded an ad with response : "
                    + _bannerView.GetResponseInfo());
            };
            // Raised when an ad fails to load into the banner view.
            _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                Debug.LogError("Banner view failed to load an ad with error : " + error);
            };
            // Raised when the ad is estimated to have earned money.
            _bannerView.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Banner view paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            _bannerView.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Banner view recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            _bannerView.OnAdClicked += () =>
            {
                Debug.Log("Banner view was clicked.");
            };
            // Raised when an ad opened full screen content.
            _bannerView.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Banner view full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            _bannerView.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Banner view full screen content closed.");
                LoadAd();
            };
        }
    }
}
