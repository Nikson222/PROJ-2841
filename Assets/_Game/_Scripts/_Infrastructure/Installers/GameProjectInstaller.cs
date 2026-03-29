using _Scripts._Infrastructure.SceneManagement;
using _Scripts._Infrastructure.Services;
using _Scripts._Infrastructure.StateMachine;
using _Scripts.Game.Services;
using Core.Infrastructure.SaveLoad;
using LoadingCurtain;
using UnityEngine;
using Zenject;

namespace _Scripts._Infrastructure.Installers
{
    public class GameProjectInstaller : MonoInstaller
    {
        [SerializeField] private CoroutineRunner _coroutineRunnerPrefab;
        [SerializeField] private Canvas _curtainCanvas;

        [SerializeField] private bool _isCoroutineRunnerRequired = true;
        [SerializeField] private bool _isCurtainRequired = true;

        public override void InstallBindings()
        {
            BindRuntimeRefs();
            InstantiateAndBindCoroutineRunnerIfNeeded();
            InstantiateAndBindCurtainIfNeeded();
            BindCoreServices();
        }

        private void BindRuntimeRefs()
        {
            if (!Container.HasBinding<RuntimeRefs>())
                Container.Bind<RuntimeRefs>().FromInstance(new RuntimeRefs()).AsSingle();
        }

        private void InstantiateAndBindCoroutineRunnerIfNeeded()
        {
            if (!_isCoroutineRunnerRequired)
                return;

            CoroutineRunner runner = Container.InstantiatePrefabForComponent<CoroutineRunner>(_coroutineRunnerPrefab);
            Container.BindInterfacesAndSelfTo<CoroutineRunner>().FromInstance(runner).AsSingle();
        }

        private void InstantiateAndBindCurtainIfNeeded()
        {
            if (!_isCurtainRequired)
                return;

            Canvas canvas = Container.InstantiatePrefab(_curtainCanvas).GetComponent<Canvas>();
            Curtain curtain = canvas.GetComponentInChildren<Curtain>();
            Container.BindInterfacesAndSelfTo<Curtain>().FromInstance(curtain).AsSingle();
        }

        private void BindCoreServices()
        {
            Container
                .Bind<GamePauseService>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<SaveLoadService>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle();
            Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerProfile>().AsSingle();
            Container.BindInterfacesAndSelfTo<ScoreCounter>().AsSingle();
        }
    }
}
