using _Scripts._Infrastructure.Ad;
using Zenject;

namespace _Scripts._Infrastructure.Installers
{
    public class AdInstaller : MonoInstaller
    {
        [UnityEngine.SerializeField] private AdProviderType _selectedProvider;
        [UnityEngine.SerializeField] private AdConfigLibrary _configLibrary;

        public override void InstallBindings()
        {
            IAdConfig found = _configLibrary != null ? _configLibrary.GetConfigFor(_selectedProvider) : null;
            if (found == null)
                throw new System.ArgumentException("Ad config not found for selected provider");

            switch (_selectedProvider)
            {
                case AdProviderType.Unity:
                    UnityAdsConfig unityConfig = found as UnityAdsConfig;
                    if (unityConfig == null)
                        throw new System.InvalidCastException("Invalid config type for Unity provider");

                    Container.Bind<UnityAdsConfig>().FromInstance(unityConfig).AsSingle();
                    Container.Bind<IAdService>().To<UnityAdService>().AsSingle();
                    break;

                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }
    }
}