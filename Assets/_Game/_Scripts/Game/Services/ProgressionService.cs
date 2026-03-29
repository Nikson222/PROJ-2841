using _Game._Scripts.Game.Configs;
using _Scripts.Game.Services;
using UnityEngine;
using Zenject;

namespace _Game._Scripts.Game.Services
{
    public class ProgressionService : IInitializable, ITickable
    {
        private readonly ProgressionConfig _config;
        private readonly ScoreCounter _scoreCounter;
        private readonly RunnerService _runnerService;

        private float _timeSinceLastCheck;
        private float _currentMultiplier = 1f;
        private int _lastScore;

        [Inject]
        public ProgressionService(
            ProgressionConfig config,
            ScoreCounter scoreCounter,
            RunnerService runnerService)
        {
            _config = config;
            _scoreCounter = scoreCounter;
            _runnerService = runnerService;
        }

        public void Initialize()
        {
            _timeSinceLastCheck = 0f;
            _currentMultiplier = 1f;
            _lastScore = _scoreCounter.Score;

            ApplyMultiplierByScore(true);
        }

        public void Tick()
        {
            if (_config == null)
                return;

            float dt = Time.deltaTime;
            if (dt <= 0f)
                return;

            _timeSinceLastCheck += dt;
            if (_timeSinceLastCheck < _config.ScoreCheckIntervalSeconds)
                return;

            _timeSinceLastCheck = 0f;
            ApplyMultiplierByScore(false);
        }

        private void ApplyMultiplierByScore(bool force)
        {
            int score = _scoreCounter.Score;

            if (!force && score == _lastScore)
                return;

            _lastScore = score;

            float targetMultiplier = _config.GetRunnerSpeedMultiplier(score);
            if (targetMultiplier < 0.1f)
                targetMultiplier = 0.1f;

            if (Mathf.Abs(targetMultiplier - _currentMultiplier) < 0.001f)
                return;

            _currentMultiplier = targetMultiplier;
            _runnerService.SetSpeedMultiplier(_currentMultiplier);
        }
    }
}
