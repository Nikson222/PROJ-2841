using System;
using System.Collections.Generic;
using _Game._Scripts.Game.Configs;
using _Game._Scripts.Game.Models;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Game._Scripts.Game.Views
{
    public class BoardView : MonoBehaviour, IChickenBoardView
    {
        [SerializeField] private RectTransform _cellsRoot;
        [SerializeField] private CellView _cellPrefab;
        [SerializeField] private GridLayoutGroup _gridLayout;

        private BoardConfig _config;
        private VisualConfig _visualConfig;
        private HighlightConfig _highlightConfig;
        private CellView[,] _cells;

        private const float DropDuration = 0.35f;
        private const float DestroyDelay = 0.25f;

        private void Awake()
        {
            if (_gridLayout == null && _cellsRoot != null)
                _gridLayout = _cellsRoot.GetComponent<GridLayoutGroup>();
        }

        public void Initialize(BoardConfig boardConfig, VisualConfig visualConfig, HighlightConfig highlightConfig)
        {
            _config = boardConfig;
            _visualConfig = visualConfig;
            _highlightConfig = highlightConfig;

            CreateCells();
        }

        private void CreateCells()
        {
            if (_cells != null && _cells.Length > 0)
            {
                int oldColumns = _cells.GetLength(0);
                int oldRows = _cells.GetLength(1);

                for (int x = 0; x < oldColumns; x++)
                for (int y = 0; y < oldRows; y++)
                {
                    CellView existing = _cells[x, y];
                    if (existing != null)
                        Destroy(existing.gameObject);
                }
            }

            int columns = _config.Columns;
            int rows = _config.Rows;

            SetupGridLayout(columns, rows);

            _cells = new CellView[columns, rows];

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    CellView cell = Instantiate(_cellPrefab, _cellsRoot);
                    _cells[x, y] = cell;
                }
            }

            ClearHighlight();
        }

        private void SetupGridLayout(int columns, int rows)
        {
            if (_gridLayout == null || _cellsRoot == null)
                return;

            Rect rect = _cellsRoot.rect;

            float spacingX = _config != null ? _config.ChickenSpacing : _gridLayout.spacing.x;
            float spacingY = _config != null ? _config.ChickenSpacing : _gridLayout.spacing.y;
            _gridLayout.spacing = new Vector2(spacingX, spacingY);

            float totalWidth = rect.width - _gridLayout.padding.left - _gridLayout.padding.right;
            float totalHeight = rect.height - _gridLayout.padding.top - _gridLayout.padding.bottom;

            float cellWidth = (totalWidth - spacingX * (columns - 1)) / columns;
            float cellHeight = (totalHeight - spacingY * (rows - 1)) / rows;

            float size = Mathf.Min(cellWidth, cellHeight);
            if (size < 0f)
                size = 0f;

            Vector2 cellSize = new Vector2(size, size);
            _gridLayout.cellSize = cellSize;

            _gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _gridLayout.constraintCount = columns;
        }

        public void ShowChicken(BoardPosition position, ChickenColor color)
        {
            CellView cell = GetCell(position);
            if (cell == null)
                return;

            Sprite sprite = _visualConfig != null ? _visualConfig.GetSprite(color) : null;
            if (sprite == null)
            {
                cell.ShowSprite(null, false);
                return;
            }

            int columnIndex = position.X;
            Vector3 bottomPos = GetBottomCellWorldPosition(columnIndex, cell.RectTransform.position);

            cell.ShowSpriteFromWorld(sprite, bottomPos, DropDuration);
        }

        public void ClearCell(BoardPosition position)
        {
            CellView cell = GetCell(position);
            if (cell == null)
                return;

            DOVirtual.DelayedCall(DropDuration, () =>
            {
                if (cell != null)
                    cell.HideSprite(true);
            });
        }

        public void ClearCluster(IReadOnlyList<BoardPosition> positions)
        {
            if (positions == null)
                return;

            for (int i = 0; i < positions.Count; i++)
                ClearCell(positions[i]);
        }

        public void CompressColumns(IReadOnlyList<(BoardPosition from, BoardPosition to)> moves)
        {
            if (moves == null || _cells == null || moves.Count == 0)
                return;

            float startDelay = DropDuration + DestroyDelay * 0.5f;

            foreach (var move in moves)
            {
                BoardPosition from = move.from;
                BoardPosition to = move.to;

                CellView fromCell = GetCell(from);
                CellView toCell = GetCell(to);

                if (fromCell == null || toCell == null)
                    continue;

                Image icon = GetIconImage(fromCell);
                if (icon == null)
                    continue;

                Sprite spriteToMove = icon.sprite;
                Vector3 targetPos = toCell.RectTransform.position;

                icon.rectTransform.DOKill();
                icon.rectTransform
                    .DOMove(targetPos, DropDuration)
                    .SetEase(Ease.OutQuad)
                    .SetDelay(startDelay)
                    .OnComplete(() =>
                    {
                        // Синхронизировать вьюшку с моделью
                        fromCell.HideSprite(false);
                        toCell.ShowSprite(spriteToMove, false);
                    });
            }
        }

        private Image GetIconImage(CellView cell)
        {
            if (cell == null)
                return null;

            Image[] images = cell.GetComponentsInChildren<Image>();
            for (int i = 0; i < images.Length; i++)
            {
                Image img = images[i];
                if (img.transform == cell.transform)
                    continue;

                return img;
            }

            return null;
        }

        public void HighlightColumn(int columnIndex, ChickenColor color)
        {
            if (_cells == null)
                return;

            int columns = _cells.GetLength(0);
            int rows = _cells.GetLength(1);

            if (columnIndex < 0 || columnIndex >= columns)
                return;

            ClearHighlight();

            Color highlightColor = Color.white;
            if (_highlightConfig != null)
                highlightColor = _highlightConfig.GetColor(color);

            for (int y = 0; y < rows; y++)
            {
                CellView cell = _cells[columnIndex, y];
                if (cell != null)
                    cell.SetHighlight(true, highlightColor);
            }
        }

        public void ClearHighlight()
        {
            if (_cells == null)
                return;

            int columns = _cells.GetLength(0);
            int rows = _cells.GetLength(1);

            for (int x = 0; x < columns; x++)
            for (int y = 0; y < rows; y++)
            {
                CellView cell = _cells[x, y];
                if (cell != null)
                    cell.SetHighlight(false);
            }
        }

        public int GetColumnIndexByWorldPosition(Vector3 worldPosition)
        {
            if (_cells == null)
                return -1;

            int columns = _cells.GetLength(0);
            int rows = _cells.GetLength(1);

            if (columns == 0 || rows == 0)
                return -1;

            float bestDistance = float.MaxValue;
            int bestColumn = -1;

            for (int x = 0; x < columns; x++)
            {
                CellView cell = _cells[x, rows - 1];
                if (cell == null)
                    continue;

                RectTransform rectTransform = cell.RectTransform;
                Vector3 cellWorldPos = rectTransform.position;

                float distance = Mathf.Abs(worldPosition.x - cellWorldPos.x);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestColumn = x;
                }
            }

            return bestColumn;
        }

        private Vector3 GetBottomCellWorldPosition(int columnIndex, Vector3 fallback)
        {
            if (_cells == null)
                return fallback;

            int columns = _cells.GetLength(0);
            int rows = _cells.GetLength(1);

            if (columnIndex < 0 || columnIndex >= columns)
                return fallback;

            float minY = float.MaxValue;
            Vector3 result = fallback;
            bool found = false;

            for (int y = 0; y < rows; y++)
            {
                CellView cell = _cells[columnIndex, y];
                if (cell == null)
                    continue;

                Vector3 pos = cell.RectTransform.position;
                if (!found || pos.y < minY)
                {
                    minY = pos.y;
                    result = pos;
                    found = true;
                }
            }

            return result;
        }

        private CellView GetCell(BoardPosition position)
        {
            if (_cells == null)
                return null;

            int columns = _cells.GetLength(0);
            int rows = _cells.GetLength(1);

            if (position.X < 0 || position.X >= columns ||
                position.Y < 0 || position.Y >= rows)
                return null;

            return _cells[position.X, position.Y];
        }

        public void RunAfterDelay(float seconds, Action callback)
        {
            if (seconds <= 0f)
            {
                callback?.Invoke();
                return;
            }

            DOVirtual.DelayedCall(seconds, () => callback?.Invoke());
        }

        public void HighlightCluster(IReadOnlyList<BoardPosition> positions, float duration)
        {
            if (positions == null || _cells == null || positions.Count == 0)
                return;

            for (int i = 0; i < positions.Count; i++)
            {
                CellView cell = GetCell(positions[i]);
                if (cell != null)
                    cell.SetHighlight(true);
            }

            DOVirtual.DelayedCall(duration, () =>
            {
                if (_cells == null)
                    return;

                for (int i = 0; i < positions.Count; i++)
                {
                    CellView cell = GetCell(positions[i]);
                    if (cell != null)
                        cell.SetHighlight(false);
                }
            });
        }
    }
}
