using System;
using _Game._Scripts.Game.Controllers;
using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.Services;
using _Scripts._Infrastructure.UI.Base;
using Zenject;

namespace _Game._Scripts.Game
{
    public class GameOverController : IInitializable, IDisposable
    {
        private readonly BoardController _boardController;
        private readonly UIPanelService _uiPanelService;
        private readonly AudioService _audioService;

        [Inject]
        public GameOverController(
            BoardController boardController,
            UIPanelService uiPanelService,
            AudioService audioService)
        {
            _boardController = boardController;
            _uiPanelService = uiPanelService;
            _audioService = audioService;
        }

        public void Initialize()
        {
            _boardController.OnGameOver += HandleGameOver;
        }

        public void Dispose()
        {
            _boardController.OnGameOver -= HandleGameOver;
        }

        private void HandleGameOver()
        {
            _audioService.PlaySound(SoundType.GameOver);
            _uiPanelService.Open(PanelType.GameOver);
        }
    }
}