using System;
using UnityEngine.Advertisements;
using Zenject;

namespace _Scripts._Infrastructure.Ad
{
    public class UnityAdService : IAdService, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener, IInitializable
    {
        private readonly UnityAdsConfig _config;

        private string _gameId;
        private string _rewardedId;
        private string _interstitialId;

        private bool _rewardedLoaded;
        private bool _interstitialLoaded;

        public event Action<AdType> OnAdStarted;
        public event Action<AdType> OnAdCompleted;
        public event Action<AdType, string> OnAdFailed;

        [Inject]
        public UnityAdService(UnityAdsConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
#if UNITY_IOS
        _gameId = _config.IOSGameId;
        _rewardedId = _config.RewardedIOSId;
        _interstitialId = _config.InterstitialIOSId;
#elif UNITY_ANDROID
        _gameId = _config.AndroidGameId;
        _rewardedId = _config.RewardedAndroidId;
        _interstitialId = _config.InterstitialAndroidId;
#else
            _gameId = _config.AndroidGameId;
            _rewardedId = _config.RewardedAndroidId;
            _interstitialId = _config.InterstitialAndroidId;
#endif
            Advertisement.Initialize(_gameId, _config.TestMode, this);
        }

        public bool IsReady(AdType type)
        {
            return type switch
            {
                AdType.Rewarded => _rewardedLoaded,
                AdType.Interstitial => _interstitialLoaded,
                _ => false
            };
        }

        public void ShowAd(AdType type)
        {
            string placementId = GetPlacementId(type);

            if (!IsReady(type))
            {
                OnAdFailed?.Invoke(type, "Ad not ready");
                return;
            }

            Advertisement.Show(placementId, this);
            OnAdStarted?.Invoke(type);

            if (type == AdType.Rewarded)
                _rewardedLoaded = false;
            else if (type == AdType.Interstitial)
                _interstitialLoaded = false;

            Advertisement.Load(placementId, this);
        }

        private string GetPlacementId(AdType type)
        {
            return type switch
            {
                AdType.Rewarded => _rewardedId,
                AdType.Interstitial => _interstitialId,
                _ => null
            };
        }

        private AdType AdTypeFromPlacementId(string placementId)
        {
            if (placementId == _rewardedId) return AdType.Rewarded;
            if (placementId == _interstitialId) return AdType.Interstitial;
            return AdType.Banner;
        }

        public void OnInitializationComplete()
        {
            Advertisement.Load(_rewardedId, this);
            Advertisement.Load(_interstitialId, this);
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            if (placementId == _rewardedId)
                _rewardedLoaded = true;
            else if (placementId == _interstitialId)
                _interstitialLoaded = true;
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            if (placementId == _rewardedId)
                _rewardedLoaded = false;
            else if (placementId == _interstitialId)
                _interstitialLoaded = false;

            OnAdFailed?.Invoke(AdTypeFromPlacementId(placementId), message);
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            var type = AdTypeFromPlacementId(placementId);

            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
                OnAdCompleted?.Invoke(type);
            else
                OnAdFailed?.Invoke(type, "Ad was skipped or failed.");
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            var type = AdTypeFromPlacementId(placementId);
            OnAdFailed?.Invoke(type, message);
        }

        public void OnUnityAdsShowStart(string placementId) { }
        public void OnUnityAdsShowClick(string placementId) { }
    }
}
