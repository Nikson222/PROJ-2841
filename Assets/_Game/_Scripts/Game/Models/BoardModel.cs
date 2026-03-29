using System.Collections.Generic;

namespace _Game._Scripts.Game.Models
{
    public class BoardModel
    {
        private readonly int _columns;
        private readonly int _rows;
        private readonly Chicken[,] _grid;

        public int Columns => _columns;
        public int Rows => _rows;

        public BoardModel(int columns, int rows)
        {
            _columns = columns;
            _rows = rows;
            _grid = new Chicken[_columns, _rows];
        }

        public bool IsInside(BoardPosition position)
        {
            return position.X >= 0 &&
                   position.X < _columns &&
                   position.Y >= 0 &&
                   position.Y < _rows;
        }

        public Chicken GetChicken(BoardPosition position)
        {
            if (!IsInside(position))
                return null;

            return _grid[position.X, position.Y];
        }

        public void SetChicken(BoardPosition position, Chicken chicken)
        {
            if (!IsInside(position))
                return;

            _grid[position.X, position.Y] = chicken;
        }

        public bool IsEmpty(BoardPosition position)
        {
            if (!IsInside(position))
                return false;

            return _grid[position.X, position.Y] == null;
        }

        public List<BoardPosition> GetEmptyPositions()
        {
            List<BoardPosition> result = new List<BoardPosition>();

            for (int x = 0; x < _columns; x++)
            {
                for (int y = 0; y < _rows; y++)
                {
                    if (_grid[x, y] == null)
                        result.Add(new BoardPosition(x, y));
                }
            }

            return result;
        }

        public bool IsFull()
        {
            for (int x = 0; x < _columns; x++)
            {
                for (int y = 0; y < _rows; y++)
                {
                    if (_grid[x, y] == null)
                        return false;
                }
            }

            return true;
        }

        public void CompressUp()
        {
            CompressUpWithMapping();
        }

        
        public List<(BoardPosition from, BoardPosition to)> CompressUpWithMapping()
        {
            var moves = new List<(BoardPosition from, BoardPosition to)>();

            for (int x = 0; x < _columns; x++)
            {
                int writeY = 0; 

                for (int y = 0; y < _rows; y++)
                {
                    Chicken chicken = _grid[x, y];
                    if (chicken == null)
                        continue;

                    if (writeY != y)
                    {
                        _grid[x, writeY] = chicken;
                        _grid[x, y] = null;

                        var from = new BoardPosition(x, y);
                        var to = new BoardPosition(x, writeY);
                        moves.Add((from, to));
                    }

                    writeY++;
                }

                for (int y = writeY; y < _rows; y++)
                {
                    _grid[x, y] = null;
                }
            }

            return moves;
        }
    }
}
