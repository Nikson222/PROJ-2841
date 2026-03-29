using UnityEngine;
using UnityEngine.UI;
using Zenject;
using _Scripts._Infrastructure.SceneManagement;
using _Scripts._Infrastructure.Services;
using _Scripts.Game.Services;
using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.MyEditorCustoms;
using _Scripts._Infrastructure.UI.Base;

namespace _Scripts._Infrastructure.UI
{
    public class GameOverPanel : AnimatedPanel
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Text _scoreText;
        [Scene] [SerializeField] private string _menuSceneName;

        private PlayerProfile _playerProfile;
        private ScoreCounter _scoreCounter;
        private SceneLoader _sceneLoader;
        private AudioService _audioService;

        [Inject]
        public void Construct(PlayerProfile profile, ScoreCounter scoreCounter, SceneLoader loader, AudioService audioService)
        {
            _playerProfile = profile;
            _scoreCounter = scoreCounter;
            _sceneLoader = loader;
            _audioService = audioService;
        }

        private void OnEnable()
        {
            _continueButton.onClick.AddListener(OnClickContinue);
        }

        private void OnDisable()
        {
            _continueButton.onClick.RemoveListener(OnClickContinue);
        }

        public override void Open()
        {
            base.Open();
            _scoreText.text = _scoreCounter.Score.ToString();
        }

        private void OnClickContinue()
        {
            _audioService.PlaySound(SoundType.ButtonClick);
            _playerProfile.SetMaxScore(_scoreCounter.Score);
            _audioService.StopMusic();

            _sceneLoader.Load(_menuSceneName, () =>
            {
                _audioService.PlayMusic();
                _scoreCounter.Reset();
            });
        }
    }
}