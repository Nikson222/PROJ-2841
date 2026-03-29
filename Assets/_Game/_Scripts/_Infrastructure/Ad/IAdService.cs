using System;

namespace _Scripts._Infrastructure.Ad
{
    public interface IAdService
    {
        event Action<AdType> OnAdStarted;
        event Action<AdType> OnAdCompleted;
        event Action<AdType, string> OnAdFailed;

        bool IsReady(AdType type);
        void ShowAd(AdType type);
    }
}