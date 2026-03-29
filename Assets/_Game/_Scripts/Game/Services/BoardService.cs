using System.Collections.Generic;
using _Game._Scripts.Game.Configs;
using _Game._Scripts.Game.Models;

namespace _Game._Scripts.Game.Services
{
    public class BoardService
    {
        private readonly BoardModel _board;
        private readonly BoardConfig _config;

        private readonly int[] _offsetX = { 1, -1, 0, 0 };
        private readonly int[] _offsetY = { 0, 0, 1, -1 };

        public BoardModel Board => _board;
        public BoardConfig Config => _config;

        public BoardService(BoardConfig config)
        {
            _config = config;
            _board = new BoardModel(config.Columns, config.Rows);
        }

        public PlacementResultWithMoves PlaceChicken(BoardPosition position, ChickenColor color)
        {
            if (!_board.IsInside(position))
                return PlacementResultWithMoves.Failed();

            if (!_board.IsEmpty(position))
                return PlacementResultWithMoves.Failed();

            Chicken chicken = new Chicken(color);
            _board.SetChicken(position, chicken);

            List<BoardPosition> cluster = FindCluster(position);

            if (cluster.Count >= _config.MinGroupSize)
            {
                RemoveCluster(cluster);
                List<(BoardPosition from, BoardPosition to)> moves = _board.CompressUpWithMapping();
                return PlacementResultWithMoves.Match(cluster.Count, cluster, moves);
            }

            return PlacementResultWithMoves.NoMatch();
        }

        public PlacementResultWithMoves PlaceChickenInColumn(int columnIndex, ChickenColor color, out BoardPosition placedPosition)
        {
            placedPosition = default;

            if (columnIndex < 0 || columnIndex >= _board.Columns)
                return PlacementResultWithMoves.Failed();

            if (!TryGetFirstEmptyFromBottom(columnIndex, out BoardPosition target))
                return PlacementResultWithMoves.Failed();

            placedPosition = target;
            return PlaceChicken(target, color);
        }

        public bool TryGetFirstEmptyFromBottom(int columnIndex, out BoardPosition position)
        {
            position = default;

            if (columnIndex < 0 || columnIndex >= _board.Columns)
                return false;

            for (int y = 0; y < _board.Rows; y++)
            {
                BoardPosition check = new BoardPosition(columnIndex, y);
                if (_board.IsEmpty(check))
                {
                    position = check;
                    return true;
                }
            }

            return false;
        }

        public bool IsGameOver()
        {
            for (int x = 0; x < _board.Columns; x++)
            {
                bool columnFull = true;

                for (int y = 0; y < _board.Rows; y++)
                {
                    BoardPosition position = new BoardPosition(x, y);
                    if (_board.IsEmpty(position))
                    {
                        columnFull = false;
                        break;
                    }
                }

                if (columnFull)
                    return true;
            }

            return false;
        }

        private List<BoardPosition> FindCluster(BoardPosition start)
        {
            List<BoardPosition> result = new List<BoardPosition>();
            Chicken startChicken = _board.GetChicken(start);

            if (startChicken == null)
                return result;

            bool[,] visited = new bool[_board.Columns, _board.Rows];
            Queue<BoardPosition> queue = new Queue<BoardPosition>();

            queue.Enqueue(start);
            visited[start.X, start.Y] = true;

            while (queue.Count > 0)
            {
                BoardPosition current = queue.Dequeue();
                result.Add(current);

                for (int i = 0; i < 4; i++)
                {
                    int nx = current.X + _offsetX[i];
                    int ny = current.Y + _offsetY[i];

                    BoardPosition neighbour = new BoardPosition(nx, ny);

                    if (!_board.IsInside(neighbour))
                        continue;

                    if (visited[nx, ny])
                        continue;

                    Chicken neighbourChicken = _board.GetChicken(neighbour);

                    if (neighbourChicken == null)
                        continue;

                    if (neighbourChicken.Color != startChicken.Color)
                        continue;

                    visited[nx, ny] = true;
                    queue.Enqueue(neighbour);
                }
            }

            return result;
        }

        private void RemoveCluster(List<BoardPosition> positions)
        {
            for (int i = 0; i < positions.Count; i++)
                _board.SetChicken(positions[i], null);
        }
    }
}
