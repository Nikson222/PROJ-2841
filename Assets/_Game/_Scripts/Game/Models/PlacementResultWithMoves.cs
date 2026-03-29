using System.Collections.Generic;

namespace _Game._Scripts.Game.Models
{
    public readonly struct PlacementResultWithMoves
    {
        public bool Success { get; }
        public bool HasMatch { get; }
        public int RemovedCount { get; }
        public IReadOnlyList<BoardPosition> RemovedPositions { get; }
        public IReadOnlyList<(BoardPosition from, BoardPosition to)> Moves { get; }

        private PlacementResultWithMoves(
            bool success,
            bool hasMatch,
            int removedCount,
            IReadOnlyList<BoardPosition> removedPositions,
            IReadOnlyList<(BoardPosition from, BoardPosition to)> moves)
        {
            Success = success;
            HasMatch = hasMatch;
            RemovedCount = removedCount;
            RemovedPositions = removedPositions;
            Moves = moves;
        }

        public static PlacementResultWithMoves Failed() =>
            new PlacementResultWithMoves(false, false, 0, null, null);

        public static PlacementResultWithMoves NoMatch() =>
            new PlacementResultWithMoves(true, false, 0, null, null);

        public static PlacementResultWithMoves Match(
            int removedCount,
            IReadOnlyList<BoardPosition> removedPositions,
            IReadOnlyList<(BoardPosition from, BoardPosition to)> moves) =>
            new PlacementResultWithMoves(true, true, removedCount, removedPositions, moves);
    }
}

