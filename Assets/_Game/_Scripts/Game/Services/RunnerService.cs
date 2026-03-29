using System;
using System.Collections.Generic;
using _Game._Scripts.Game.Configs;
using _Game._Scripts.Game.Models;
using UnityEngine;
using Zenject;

namespace _Game._Scripts.Game.Services
{
    public class RunnerService : IInitializable, ITickable
    {
        private readonly GameConfig _config;
        private readonly VisualConfig _visualConfig;
        private readonly System.Random _random;

        private readonly List<RunnerChicken> _chickens = new List<RunnerChicken>();

        private int _nextId;

        private float _travelTimeSeconds;
        private float _spawnIntervalSeconds;
        private float _speedPerSecond;
        private float _timeSinceLastSpawn;
        private float _spacingOffset;

        private float _baseTravelTimeSeconds;
        private float _baseSpawnIntervalSeconds;
        private float _currentSpeedMultiplier = 1f;

        private IReadOnlyList<ChickenColor> _availableColors;

        public event Action<RunnerChicken> OnChickenSpawned;
        public event Action<int, float> OnChickenProgressChanged;
        public event Action<RunnerChicken> OnChickenReachedEnd;

        public IReadOnlyList<RunnerChicken> Chickens => _chickens;

        [Inject]
        public RunnerService(
            GameConfig config,
            VisualConfig visualConfig,
            System.Random random)
        {
            _config = config;
            _visualConfig = visualConfig;
            _random = random;
        }

        public void Initialize()
        {
            _chickens.Clear();
            _nextId = 0;

            if (_visualConfig != null)
                _availableColors = _visualConfig.GetAvailableColors();
            else
                _availableColors = null;

            if (_availableColors == null || _availableColors.Count == 0)
            {
                Debug.LogError("RunnerService: нет доступных цветов в VisualConfig.");
                return;
            }

            float cfgTime = _config.RunnerTravelTimeSeconds;
            if (cfgTime <= 0.0f || float.IsNaN(cfgTime) || float.IsInfinity(cfgTime))
                cfgTime = 6.0f;

            _baseTravelTimeSeconds = cfgTime;
            _currentSpeedMultiplier = 1f;
            _travelTimeSeconds = _baseTravelTimeSeconds;
            _speedPerSecond = 1.0f / _travelTimeSeconds;

            _timeSinceLastSpawn = 0.0f;

            _spacingOffset = Mathf.Max(0f, _config.RunnerSpacingOffset);

            // Дефолтный интервал до калибровки по layout
            float defaultNormalizedWidth = 0.2f;
            _spawnIntervalSeconds = _travelTimeSeconds * defaultNormalizedWidth;
            _baseSpawnIntervalSeconds = _spawnIntervalSeconds;

            SpawnChicken();
        }

        public void Tick()
        {
            if (_availableColors == null || _availableColors.Count == 0)
                return;

            float dt = Time.deltaTime;
            if (dt <= 0.0f)
                return;

            _timeSinceLastSpawn += dt;
            if (_timeSinceLastSpawn >= _spawnIntervalSeconds)
            {
                _timeSinceLastSpawn = 0.0f;
                SpawnChicken();
            }

            if (_chickens.Count == 0)
                return;

            List<RunnerChicken> reachedEnd = null;

            for (int i = 0; i < _chickens.Count; i++)
            {
                RunnerChicken chicken = _chickens[i];

                if (chicken.IsGrabbed)
                    continue;

                chicken.Progress += _speedPerSecond * dt;

                if (chicken.Progress > 1.0f)
                    chicken.Progress = 1.0f;

                OnChickenProgressChanged?.Invoke(chicken.Id, chicken.Progress);

                if (chicken.Progress >= 1.0f)
                {
                    if (reachedEnd == null)
                        reachedEnd = new List<RunnerChicken>();

                    reachedEnd.Add(chicken);
                }
            }

            if (reachedEnd == null)
                return;

            for (int i = 0; i < reachedEnd.Count; i++)
            {
                RunnerChicken chicken = reachedEnd[i];
                _chickens.Remove(chicken);
                OnChickenReachedEnd?.Invoke(chicken);
            }
        }

        public void ConfigureSpawnIntervalByLayout(float trackWidth, float chickenWidth)
        {
            if (trackWidth <= 0.0f || chickenWidth <= 0.0f)
                return;

            float normalizedWidth = chickenWidth / trackWidth;

            float spacingNormalized = normalizedWidth * (1.0f + _spacingOffset);

            float interval = spacingNormalized * _travelTimeSeconds;

            if (interval <= 0.01f || float.IsNaN(interval) || float.IsInfinity(interval))
                return;

            _baseSpawnIntervalSeconds = interval;
            _spawnIntervalSeconds = _baseSpawnIntervalSeconds / _currentSpeedMultiplier;

            if (_timeSinceLastSpawn > _spawnIntervalSeconds)
                _timeSinceLastSpawn = _spawnIntervalSeconds;
        }

        public RunnerChicken GetChickenById(int id)
        {
            for (int i = 0; i < _chickens.Count; i++)
            {
                if (_chickens[i].Id == id)
                    return _chickens[i];
            }

            return null;
        }

        public void ConsumeChicken(int id)
        {
            for (int i = 0; i < _chickens.Count; i++)
            {
                if (_chickens[i].Id == id)
                {
                    _chickens.RemoveAt(i);
                    return;
                }
            }
        }

        public void SetChickenGrabbed(int id, bool isGrabbed)
        {
            RunnerChicken chicken = GetChickenById(id);
            if (chicken == null)
                return;

            chicken.IsGrabbed = isGrabbed;
        }

        public void SetChickenProgress(int id, float progress)
        {
            RunnerChicken chicken = GetChickenById(id);
            if (chicken == null)
                return;

            if (progress < 0f)
                progress = 0f;
            else if (progress > 1f)
                progress = 1f;

            chicken.Progress = progress;
            OnChickenProgressChanged?.Invoke(chicken.Id, chicken.Progress);
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            if (multiplier < 0.1f)
                multiplier = 0.1f;

            _currentSpeedMultiplier = multiplier;

            if (_baseTravelTimeSeconds <= 0.0f)
                _baseTravelTimeSeconds = 6.0f;

            _travelTimeSeconds = _baseTravelTimeSeconds / _currentSpeedMultiplier;
            _speedPerSecond = 1.0f / _travelTimeSeconds;

            if (_baseSpawnIntervalSeconds > 0.0f)
                _spawnIntervalSeconds = _baseSpawnIntervalSeconds / _currentSpeedMultiplier;
        }

        private void SpawnChicken()
        {
            if (_availableColors == null || _availableColors.Count == 0)
                return;

            int index = _random.Next(0, _availableColors.Count);
            ChickenColor color = _availableColors[index];

            RunnerChicken chicken = new RunnerChicken(_nextId++, color);
            _chickens.Add(chicken);

            OnChickenSpawned?.Invoke(chicken);
            OnChickenProgressChanged?.Invoke(chicken.Id, chicken.Progress);
        }
    }
}
