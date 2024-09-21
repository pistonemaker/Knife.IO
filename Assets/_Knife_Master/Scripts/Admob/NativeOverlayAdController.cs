using System;
using UnityEngine;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Sample
{
    public class NativeOverlayAdController : MonoBehaviour
    {
#if UNITY_ANDROID
        //private const string _adUnitId = "ca-app-pub-3940256099942544/2247696110";
        private const string _adUnitId = "ca-app-pub-4608150884660102/4488182293";
#elif UNITY_IPHONE
        private const string _adUnitId = "ca-app-pub-3940256099942544/3986624511";
#else
        private const string _adUnitId = "unused";
#endif

        public RectTransform AdPlacmentTarget;

        // Define our native ad advanced options.
        public NativeAdOptions Option = new NativeAdOptions
        {
            AdChoicesPlacement = AdChoicesPlacement.BottomLeftCorner,
            MediaAspectRatio = MediaAspectRatio.Portrait,
        };

        // Define our native ad template style.
        public NativeTemplateStyle Style = new NativeTemplateStyle
        {
            TemplateId = NativeTemplateId.Small,
        };

        private NativeOverlayAd _nativeOverlayAd;
        public Action OnAdsClose;
        
        public bool isLoaded = false;
        public bool isSucess = false;

        // Loads the ad.
        public void LoadAd()
        {
            // Clean up the old ad before loading a new one.
            if (_nativeOverlayAd != null)
            {
                DestroyAd();
            }

            Debug.Log("Loading native overlay ad.");

            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // Send the request to load the ad.
            NativeOverlayAd.Load(_adUnitId, adRequest, Option,
                (NativeOverlayAd ad, LoadAdError error) =>
            {
                // If the operation failed with a reason.
                if (error != null)
                {
                    isLoaded = false;
                    Debug.LogError("Native Overlay ad failed to load an ad with error : " + error);
                    return;
                }
                
                // If the operation failed for unknown reasons.
                // This is an unexpected error, please report this bug if it happens.
                if (ad == null)
                {
                    isLoaded = false;
                    Debug.LogError("Unexpected error: Native Overlay ad load event fired with " +
                    " null ad and null error.");
                    return;
                }

                // The operation completed successfully.
                Debug.Log("Native Overlay ad loaded with response : " + ad.GetResponseInfo());
                _nativeOverlayAd = ad;
                isLoaded = true;

                // Register to ad events to extend functionality.
                RegisterEventHandlers(ad);
            });
        }

        // Shows the ad.
        public void ShowAd()
        {
            if (_nativeOverlayAd != null)
            {
                RenderAd();
                isSucess = true;
                Debug.Log("Showing Native Overlay ad.");
                _nativeOverlayAd.Show();
            }
            else
            {
                isSucess = false;
                Debug.LogError("Native Overlay ad is not ready yet.");
            }
        }

        // Hides the ad.
        public void HideAd()
        {
            if (_nativeOverlayAd != null)
            {
                Debug.Log("Hiding Native Overlay ad.");
                _nativeOverlayAd.Hide();
            }
        }

        // Renders the ad.
        public void RenderAd()
        {
            if (_nativeOverlayAd != null)
            {
                Debug.Log("Rendering Native Overlay ad.");

                // Renders a native overlay ad at the default size
                // and anchored to the bottom of the screne.
                _nativeOverlayAd.RenderTemplate(Style, AdPosition.Bottom);
            }
        }

        /// Destroys the ad.
        /// When you are finished with the ad, make sure to call the Destroy()
        /// method before dropping your reference to it.
        public void DestroyAd()
        {
            if (_nativeOverlayAd != null)
            {
                isLoaded = false;
                isSucess = false;
                Debug.Log("Destroying Native Overlay ad.");
                _nativeOverlayAd.Destroy();
                _nativeOverlayAd = null;
            }
        }

        // Logs the ResponseInfo.
        public void LogResponseInfo()
        {
            if (_nativeOverlayAd != null)
            {
                var responseInfo = _nativeOverlayAd.GetResponseInfo();
                if (responseInfo != null)
                {
                    Debug.Log(responseInfo);
                }
            }
        }

        private void RegisterEventHandlers(NativeOverlayAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Native Overlay ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Native Overlay ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("Native Overlay ad was clicked.");
            };
            // Raised when the ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Native Overlay ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Native Overlay ad full screen content closed.");
                LoadAd();
                RenderAd();
            };
        }
    }
}
