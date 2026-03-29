using System;
using System.Collections.Generic;
using _Game._Scripts.Game.Configs;
using _Game._Scripts.Game.Models;
using _Game._Scripts.Game.Services;
using _Game._Scripts.Game.Views;
using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.Services;
using _Scripts.Game.Services;

namespace _Game._Scripts.Game.Controllers
{
    public class BoardController
    {
        private readonly BoardService _boardService;
        private readonly ScoreCounter _scoreCounter;
        private readonly BoardConfig _boardConfig;
        private readonly GameConfig _gameConfig;
        private readonly IChickenBoardView _boardView;
        private readonly AudioService _audioService;

        private bool _isGameOver;
        private bool _isBusy;

        public event Action<IReadOnlyList<BoardPosition>, int> OnClusterCleared;
        public event Action OnGameOver;
        public event Action<bool> OnBusyChanged;

        public bool IsGameOver => _isGameOver;
        public bool IsBusy => _isBusy;

        public BoardController(
            BoardService boardService,
            ScoreCounter scoreCounter,
            BoardConfig boardConfig,
            GameConfig gameConfig,
            IChickenBoardView boardView,
            AudioService audioService)
        {
            _boardService = boardService;
            _scoreCounter = scoreCounter;
            _boardConfig = boardConfig;
            _gameConfig = gameConfig;
            _boardView = boardView;
            _audioService = audioService;
        }

        public bool TryPlaceChickenInColumn(
            ChickenColor color,
            int columnIndex,
            out BoardPosition placedPosition)
        {
            placedPosition = default;

            if (_isGameOver || _isBusy)
                return false;

            PlacementResultWithMoves result = _boardService
                .PlaceChickenInColumn(columnIndex, color, out placedPosition);

            if (!result.Success)
                return false;

            _boardView.ShowChicken(placedPosition, color);
            HandlePlacementResult(result);

            return true;
        }

        public bool TryPlaceChickenAt(
            ChickenColor color,
            BoardPosition position)
        {
            if (_isGameOver || _isBusy)
                return false;

            PlacementResultWithMoves result = _boardService.PlaceChicken(position, color);

            if (!result.Success)
                return false;

            _boardView.ShowChicken(position, color);
            HandlePlacementResult(result);

            return true;
        }

        private void HandlePlacementResult(PlacementResultWithMoves result)
        {
            // Звук «поставили курицу на поле» – срабатывает при любом успешном размещении
            _audioService.PlaySound(SoundType.Swap);

            if (result.HasMatch)
            {
                // Звук клира кластера – обычный или «сочный» в зависимости от размера
                PlayMatchSound(result.RemovedCount);

                int scoreDelta = CalculateScore(result.RemovedCount);
                bool isBonus = result.RemovedCount > _boardConfig.MinGroupSize;

                _scoreCounter.AddScore(scoreDelta, isBonus);

                SetBusy(true);

                const float clusterHighlightDurationAfterDrop = 0.3f;
                _boardView.HighlightCluster(result.RemovedPositions, clusterHighlightDurationAfterDrop);

                _boardView.ClearCluster(result.RemovedPositions);

                if (result.Moves != null && result.Moves.Count > 0)
                    _boardView.CompressColumns(result.Moves);

                OnClusterCleared?.Invoke(result.RemovedPositions, scoreDelta);

                const float safetyDelaySeconds = 1.2f;
                _boardView.RunAfterDelay(safetyDelaySeconds, () =>
                {
                    SetBusy(false);

                    if (_boardService.IsGameOver())
                        SetGameOver();
                });
                return;
            }

            if (_boardService.IsGameOver())
                SetGameOver();
        }

        private int CalculateScore(int removedCount)
        {
            int score = _gameConfig.BaseScorePerChicken * removedCount;

            if (removedCount > _boardConfig.MinGroupSize &&
                _gameConfig.BonusPerExtraChicken > 0)
            {
                int extra = removedCount - _boardConfig.MinGroupSize;
                score += _gameConfig.BonusPerExtraChicken * extra;
            }

            return score;
        }

        private void PlayMatchSound(int removedCount)
        {
            if (removedCount >= _boardConfig.MinGroupSize + 3)
            {
                _audioService.PlaySound(SoundType.Pickup2);
            }
            else
            {
                _audioService.PlaySound(SoundType.Pickup1);
            }
        }

        private void SetGameOver()
        {
            if (_isGameOver)
                return;

            _isGameOver = true;
            OnGameOver?.Invoke();
        }

        private void SetBusy(bool value)
        {
            if (_isBusy == value)
                return;

            _isBusy = value;
            OnBusyChanged?.Invoke(_isBusy);
        }
    }
}
