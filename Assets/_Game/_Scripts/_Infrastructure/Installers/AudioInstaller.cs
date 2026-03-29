using _Scripts._Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace _Scripts._Infrastructure.Installers
{
    public class AudioInstaller : MonoInstaller
    {
        [SerializeField] private Configs.AudioConfig _audioConfig;

        public override void InstallBindings()
        {
            BindConfig();
            CreateAndBindAudio();
            BindServices();
        }

        private void BindConfig()
        {
            if (_audioConfig == null)
                throw new System.ArgumentNullException(nameof(_audioConfig));

            Container
                .Bind<Configs.AudioConfig>()
                .FromInstance(_audioConfig)
                .AsSingle();
        }

        private void CreateAndBindAudio()
        {
            GameObject parent = new("AudioSources");
            DontDestroyOnLoad(parent);

            GameObject soundGo = new("SoundSource");
            GameObject musicGo = new("MusicSource");

            soundGo.transform.SetParent(parent.transform, false);
            musicGo.transform.SetParent(parent.transform, false);

            AudioSource soundSource = soundGo.AddComponent<AudioSource>();
            AudioSource musicSource = musicGo.AddComponent<AudioSource>();
            musicSource.loop = true;

            RuntimeRefs refs = GetOrBindRuntimeRefs();
            refs.SetAudioSources(soundSource, musicSource);
        }

        private void BindServices()
        {
            Container
                .BindInterfacesAndSelfTo<AudioService>()
                .AsSingle();
        }

        private RuntimeRefs GetOrBindRuntimeRefs()
        {
            if (Container.HasBinding<RuntimeRefs>())
                return Container.Resolve<RuntimeRefs>();

            RuntimeRefs created = new();
            Container.Bind<RuntimeRefs>().FromInstance(created).AsSingle();
            return created;
        }
    }
}
