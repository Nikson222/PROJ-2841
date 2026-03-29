using System;

namespace _Scripts.Game.Services
{
    public class ScoreCounter
    {
        private int _score;
        public int Score => _score;

        public event Action<int, bool> OnScoreChanged;
        
        public void Reset()
        {
            _score = 0;
            OnScoreChanged?.Invoke(_score, false);
        }

        public void AddScore(int score, bool isBonusScore = false)
        {
            _score += score;
            OnScoreChanged?.Invoke(_score, isBonusScore);
        }
    }
}