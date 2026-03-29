using System.Collections.Generic;
using UnityEngine;

namespace _Scripts._Infrastructure.Ad
{
    [CreateAssetMenu(fileName = "AdConfigLibrary", menuName = "Configs/Ad Config Library")]
    public class AdConfigLibrary : ScriptableObject
    {
        public List<AdConfigBase> Configs = new();

        public IAdConfig GetConfigFor(AdProviderType type)
        {
            foreach (AdConfigBase config in Configs)
            {
                if (config != null && config.ProviderType == type)
                    return config;
            }

            return null;
        }
    }
}