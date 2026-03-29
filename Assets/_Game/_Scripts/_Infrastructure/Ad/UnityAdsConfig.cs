using UnityEngine;

namespace _Scripts._Infrastructure.Ad
{
    [CreateAssetMenu(fileName = "UnityAdsConfig", menuName = "Configs/Unity Ads Config")]
    public class UnityAdsConfig : AdConfigBase
    {
        public string AndroidGameId;
        public string IOSGameId;
        public string RewardedAndroidId;
        public string RewardedIOSId;
        public string InterstitialAndroidId;
        public string InterstitialIOSId;
        public bool TestMode;

        public override AdProviderType ProviderType => AdProviderType.Unity;
    }
}