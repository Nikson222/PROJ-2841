using UnityEngine;

namespace _Scripts._Infrastructure.Services
{
    public class GamePauseService
    {
        private int _pauseRequests;
        private float _previousTimeScale = 1f;

        public bool IsPaused => _pauseRequests > 0;

        public void RequestPause()
        {
            if (_pauseRequests == 0)
            {
                _previousTimeScale = Time.timeScale;
                if (_previousTimeScale <= 0f)
                    _previousTimeScale = 1f;

                Time.timeScale = 0f;
            }

            _pauseRequests++;
        }

        public void ReleasePause()
        {
            if (_pauseRequests == 0)
                return;

            _pauseRequests--;

            if (_pauseRequests <= 0)
            {
                _pauseRequests = 0;
                Time.timeScale = _previousTimeScale;
            }
        }

        public void ForceResume()
        {
            _pauseRequests = 0;
            if (Time.timeScale == 0f)
                Time.timeScale = _previousTimeScale > 0f ? _previousTimeScale : 1f;
        }
    }
}