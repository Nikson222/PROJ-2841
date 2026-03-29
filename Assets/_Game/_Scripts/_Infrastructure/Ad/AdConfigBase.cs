using UnityEngine;

namespace _Scripts._Infrastructure.Ad
{
    public abstract class AdConfigBase : ScriptableObject, IAdConfig
    {
        public abstract AdProviderType ProviderType { get; }
    }
}