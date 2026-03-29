using System;
using System.Collections.Generic;
using _Game._Scripts.Game.Services;
using UnityEngine;

namespace _Game._Scripts.Game.Configs
{
    [CreateAssetMenu(fileName = "ChickenProgressionConfig", menuName = "Configs/ChickenProgressionConfig")]
    public class ProgressionConfig : ScriptableObject
    {
        [Serializable]
        public class RunnerSpeedStep
        {
            [Min(0)]
            public int ScoreThreshold;

            [Min(0.1f)]
            public float SpeedMultiplier = 1f;
        }

        [SerializeField] private List<RunnerSpeedStep> _runnerSpeedSteps = new List<RunnerSpeedStep>();

        [Tooltip("Как часто пересчитывать прогрессию по очкам, сек.")]
        [Min(0.01f)]
        public float ScoreCheckIntervalSeconds = 0.1f;

        public float GetRunnerSpeedMultiplier(int score)
        {
            float result = 1f;

            if (_runnerSpeedSteps == null || _runnerSpeedSteps.Count == 0)
                return result;

            for (int i = 0; i < _runnerSpeedSteps.Count; i++)
            {
                RunnerSpeedStep step = _runnerSpeedSteps[i];
                if (step == null)
                    continue;

                if (score >= step.ScoreThreshold && step.SpeedMultiplier > result)
                    result = step.SpeedMultiplier;
            }

            return result;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_runnerSpeedSteps == null)
                return;

            _runnerSpeedSteps.Sort((a, b) =>
            {
                if (a == null && b == null)
                    return 0;
                if (a == null)
                    return 1;
                if (b == null)
                    return -1;
                return a.ScoreThreshold.CompareTo(b.ScoreThreshold);
            });
        }
#endif
    }
}