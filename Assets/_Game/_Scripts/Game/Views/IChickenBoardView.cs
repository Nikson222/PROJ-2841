using System;
using System.Collections.Generic;
using _Game._Scripts.Game.Models;
using UnityEngine;

namespace _Game._Scripts.Game.Views
{
    public interface IChickenBoardView
    {
        void ShowChicken(BoardPosition position, ChickenColor color);
        void ClearCluster(IReadOnlyList<BoardPosition> positions);
        void CompressColumns(IReadOnlyList<(BoardPosition from, BoardPosition to)> moves);

        void HighlightCluster(IReadOnlyList<BoardPosition> positions, float duration);

        void HighlightColumn(int columnIndex, ChickenColor color);
        void ClearHighlight();

        int GetColumnIndexByWorldPosition(Vector3 worldPosition);

        void RunAfterDelay(float seconds, Action callback);
    }
}