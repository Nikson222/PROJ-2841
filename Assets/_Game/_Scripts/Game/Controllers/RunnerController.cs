using System;
using System.Collections.Generic;
using _Game._Scripts.Game.Configs;
using _Game._Scripts.Game.Models;
using _Game._Scripts.Game.Services;
using _Game._Scripts.Game.Views;
using _Scripts.Game.Services;
using UnityEngine;
using Zenject;

namespace _Game._Scripts.Game.Controllers
{
    public class RunnerController : IInitializable, IDisposable
    {
        private readonly RunnerService _runnerService;
        private readonly BoardController _boardController;
        private readonly BoardService _boardService;
        private readonly BoardConfig _boardConfig;
        private readonly IRunnerView _runnerView;
        private readonly IChickenBoardView _boardView;
        private readonly System.Random _random;

        private readonly Queue<PendingPlacement> _pendingPlacements = new Queue<PendingPlacement>();

        private bool _spawnIntervalConfigured;

        private struct PendingPlacement
        {
            public ChickenColor Color { get; }
            public int? ColumnIndex { get; }

            public bool HasColumn => ColumnIndex.HasValue;

            public PendingPlacement(ChickenColor color, int? columnIndex)
            {
                Color = color;
                ColumnIndex = columnIndex;
            }
        }

        public RunnerController(
            RunnerService runnerService,
            BoardController boardController,
            BoardService boardService,
            BoardConfig boardConfig,
            IRunnerView runnerView,
            IChickenBoardView boardView,
            System.Random random)
        {
            _runnerService = runnerService;
            _boardController = boardController;
            _boardService = boardService;
            _boardConfig = boardConfig;
            _runnerView = runnerView;
            _boardView = boardView;
            _random = random;
        }

        public void Initialize()
        {
            _runnerView.Initialize(_boardConfig.Columns);

            _runnerView.OnChickenBeginDrag += OnChickenBeginDrag;
            _runnerView.OnChickenEndDrag += OnChickenEndDrag;
            _runnerView.OnChickenDragOver += OnChickenDragOver;
            _runnerView.OnChickenDropped += OnChickenDropped;

            _runnerService.OnChickenSpawned += OnChickenSpawned;
            _runnerService.OnChickenProgressChanged += OnChickenProgressChanged;
            _runnerService.OnChickenReachedEnd += OnChickenReachedEnd;

            _boardController.OnBusyChanged += OnBoardBusyChanged;

            // ВАЖНО: досоздаём вью для куриц, которые могли появиться ДО подписки на события.
            IReadOnlyList<RunnerChicken> chickens = _runnerService.Chickens;
            if (chickens != null)
            {
                for (int i = 0; i < chickens.Count; i++)
                {
                    RunnerChicken chicken = chickens[i];
                    // Принудительно прогоняем через тот же обработчик, что и обычный спавн
                    OnChickenSpawned(chicken);
                }
            }
        }

        public void Dispose()
        {
            _runnerView.OnChickenBeginDrag -= OnChickenBeginDrag;
            _runnerView.OnChickenEndDrag -= OnChickenEndDrag;
            _runnerView.OnChickenDragOver -= OnChickenDragOver;
            _runnerView.OnChickenDropped -= OnChickenDropped;

            _runnerService.OnChickenSpawned -= OnChickenSpawned;
            _runnerService.OnChickenProgressChanged -= OnChickenProgressChanged;
            _runnerService.OnChickenReachedEnd -= OnChickenReachedEnd;

            _boardController.OnBusyChanged -= OnBoardBusyChanged;
        }

        private void OnBoardBusyChanged(bool isBusy)
        {
            if (!isBusy)
                TryProcessPendingPlacements();
        }

        private void OnChickenSpawned(RunnerChicken chicken)
        {
            _runnerView.SpawnChicken(chicken.Id, chicken.Color, chicken.Progress);

            if (!_spawnIntervalConfigured)
            {
                ConfigureRunnerSpawnInterval(chicken.Id);
                _spawnIntervalConfigured = true;
            }
        }

        private void OnChickenProgressChanged(int id, float progress)
        {
            _runnerView.UpdateChickenProgress(id, progress);
            TryProcessPendingPlacements();
        }

        private void OnChickenReachedEnd(RunnerChicken chicken)
        {
            if (chicken == null)
                return;

            EnqueuePlacement(chicken.Color, null);
            _runnerView.RemoveChicken(chicken.Id);
            TryProcessPendingPlacements();
        }

        private void OnChickenBeginDrag(int id)
        {
            _runnerService.SetChickenGrabbed(id, true);
        }

        private void OnChickenEndDrag(int id)
        {
            _runnerService.SetChickenGrabbed(id, false);
        }

        private void OnChickenDragOver(int id, Vector3 worldPosition, bool isUpperHalf)
        {
            if (!isUpperHalf)
            {
                _boardView.ClearHighlight();
                return;
            }

            RunnerChicken chicken = _runnerService.GetChickenById(id);
            if (chicken == null)
            {
                _boardView.ClearHighlight();
                return;
            }

            int columnIndex = _boardView.GetColumnIndexByWorldPosition(worldPosition);

            if (columnIndex < 0 || columnIndex >= _boardConfig.Columns)
            {
                _boardView.ClearHighlight();
                return;
            }

            _boardView.HighlightColumn(columnIndex, chicken.Color);
        }

        private void OnChickenDropped(int id, Vector3 worldPosition, bool isUpperHalf, float progress)
        {
            _boardView.ClearHighlight();

            RunnerChicken chicken = _runnerService.GetChickenById(id);
            if (chicken == null)
            {
                _runnerView.RemoveChicken(id);
                return;
            }

            if (_boardController.IsGameOver)
            {
                _runnerService.ConsumeChicken(id);
                _runnerView.RemoveChicken(id);
                return;
            }

            int? columnIndex = null;

            if (isUpperHalf)
            {
                int col = _boardView.GetColumnIndexByWorldPosition(worldPosition);
                if (col >= 0 && col < _boardConfig.Columns)
                    columnIndex = col;
            }

            EnqueuePlacement(chicken.Color, columnIndex);

            _runnerService.ConsumeChicken(id);
            _runnerView.RemoveChicken(id);

            TryProcessPendingPlacements();
        }

        private void EnqueuePlacement(ChickenColor color, int? columnIndex)
        {
            PendingPlacement placement = new PendingPlacement(color, columnIndex);
            _pendingPlacements.Enqueue(placement);
        }

        private void TryProcessPendingPlacements()
        {
            if (_boardController.IsBusy)
                return;

            if (_pendingPlacements.Count == 0)
                return;

            PendingPlacement placement = _pendingPlacements.Dequeue();
            ExecutePlacement(placement);
        }

        private void ExecutePlacement(PendingPlacement placement)
        {
            bool placed = false;

            if (placement.HasColumn)
            {
                BoardPosition position;
                placed = _boardController.TryPlaceChickenInColumn(
                    placement.Color,
                    placement.ColumnIndex.Value,
                    out position);
            }

            if (placed)
                return;

            List<int> availableColumns = GetColumnsWithEmptyCells();
            if (availableColumns.Count == 0)
                return;

            int randomIndex = _random.Next(0, availableColumns.Count);
            int columnIndex = availableColumns[randomIndex];

            BoardPosition pos;
            _boardController.TryPlaceChickenInColumn(placement.Color, columnIndex, out pos);
        }

        private List<int> GetColumnsWithEmptyCells()
        {
            List<int> result = new List<int>();

            List<BoardPosition> emptyPositions = _boardService.Board.GetEmptyPositions();
            for (int i = 0; i < emptyPositions.Count; i++)
            {
                int x = emptyPositions[i].X;

                bool alreadyAdded = false;
                for (int j = 0; j < result.Count; j++)
                {
                    if (result[j] == x)
                    {
                        alreadyAdded = true;
                        break;
                    }
                }

                if (!alreadyAdded)
                    result.Add(x);
            }

            return result;
        }

        private void ConfigureRunnerSpawnInterval(int sampleChickenId)
        {
            float trackWidth = _runnerView.GetTrackWidth();
            float chickenWidth = _runnerView.GetChickenWidth(sampleChickenId);

            if (trackWidth <= 0.0f || chickenWidth <= 0.0f)
                return;

            _runnerService.ConfigureSpawnIntervalByLayout(trackWidth, chickenWidth);
        }
    }
}
