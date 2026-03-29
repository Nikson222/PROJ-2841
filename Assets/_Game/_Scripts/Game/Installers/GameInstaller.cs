using System;
using _Game._Scripts.Game;
using _Game._Scripts.Game.Configs;
using _Game._Scripts.Game.Controllers;
using _Game._Scripts.Game.Services;
using _Game._Scripts.Game.Views;
using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.Services;
using _Scripts._Infrastructure.UI.Base;
using UnityEngine;
using Zenject;

namespace _Game._Scripts.Game.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private Canvas _levelCanvas;
        [SerializeField] private BoardConfig _boardConfig;
        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private VisualConfig _visualConfig;
        [SerializeField] private HighlightConfig _highlightConfig;
        [SerializeField] private ProgressionConfig _progressionConfig;
        [SerializeField] private BoardView _boardViewPrefab;

        public override void InstallBindings()
        {
            BindConfigs();
            BindRandom();

            BoardView boardView = CreateBoardView();
            RunnerView runnerView = FindRunnerView(boardView);

            BindViews(boardView, runnerView);
            BindServicesAndControllers();
        }

        private void BindConfigs()
        {
            if (_levelCanvas == null)
                throw new ArgumentNullException(nameof(_levelCanvas));
            if (_boardConfig == null)
                throw new ArgumentNullException(nameof(_boardConfig));
            if (_gameConfig == null)
                throw new ArgumentNullException(nameof(_gameConfig));
            if (_visualConfig == null)
                throw new ArgumentNullException(nameof(_visualConfig));
            if (_highlightConfig == null)
                throw new ArgumentNullException(nameof(_highlightConfig));
            if (_progressionConfig == null)
                throw new ArgumentNullException(nameof(_progressionConfig));

            Container
                .Bind<Canvas>()
                .WithId("LevelCanvas")
                .FromInstance(_levelCanvas)
                .AsSingle();

            Container
                .Bind<BoardConfig>()
                .FromInstance(_boardConfig)
                .AsSingle();

            Container
                .Bind<GameConfig>()
                .FromInstance(_gameConfig)
                .AsSingle();

            Container
                .Bind<VisualConfig>()
                .FromInstance(_visualConfig)
                .AsSingle();

            Container
                .Bind<HighlightConfig>()
                .FromInstance(_highlightConfig)
                .AsSingle();

            Container
                .Bind<ProgressionConfig>()
                .FromInstance(_progressionConfig)
                .AsSingle();
        }

        private void BindRandom()
        {
            Container
                .Bind<System.Random>()
                .AsSingle();
        }

        private BoardView CreateBoardView()
        {
            if (_boardViewPrefab == null)
                throw new ArgumentNullException(nameof(_boardViewPrefab));

            Canvas canvas = Container.ResolveId<Canvas>("LevelCanvas");
            Transform parent = canvas.transform;

            BoardView boardView =
                Container.InstantiatePrefabForComponent<BoardView>(_boardViewPrefab, parent);

            Container.InjectGameObject(boardView.gameObject);

            boardView.Initialize(_boardConfig, _visualConfig, _highlightConfig);
            boardView.gameObject.SetActive(true);

            return boardView;
        }

        private RunnerView FindRunnerView(BoardView boardView)
        {
            RunnerView runnerView = boardView.GetComponentInChildren<RunnerView>(true);

            if (runnerView == null)
                throw new InvalidOperationException("RunnerView not found inside BoardView prefab.");

            return runnerView;
        }

        private void BindViews(BoardView boardView, RunnerView runnerView)
        {
            Container
                .Bind<IChickenBoardView>()
                .FromInstance(boardView)
                .AsSingle();

            Container
                .Bind<IRunnerView>()
                .FromInstance(runnerView)
                .AsSingle();
        }

        private void BindServicesAndControllers()
        {
            Container
                .BindInterfacesAndSelfTo<BoardService>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<RunnerService>()
                .AsSingle();

            Container
                .Bind<BoardController>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<RunnerController>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<GameOverController>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<ProgressionService>()
                .AsSingle();
        }
    }
}
