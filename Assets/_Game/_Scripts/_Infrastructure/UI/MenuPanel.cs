using _Scripts._Infrastructure.Configs;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using _Scripts._Infrastructure.SceneManagement;
using _Scripts._Infrastructure.Services;
using _Scripts._Infrastructure.UI.Base;

namespace _Scripts._Infrastructure.UI
{
    public class MenuPanel : AnimatedPanel
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;

        private AudioService _audioService;
        private UIPanelService _uiPanelService;
        private SceneLoader _sceneLoader;

        [Inject]
        public void Construct(SceneLoader sceneLoader, AudioService audioService, UIPanelService uiPanelService)
        {
            _sceneLoader = sceneLoader;
            _audioService = audioService;
            _uiPanelService = uiPanelService;
        }

        private void OnEnable()
        {
            _audioService.PlayMusic();
            _playButton.onClick.AddListener(OnClickPlay);
            _settingsButton.onClick.AddListener(OnClickSettings);
            _exitButton.onClick.AddListener(Application.Quit);
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveAllListeners();
            _settingsButton.onClick.RemoveAllListeners();
            _exitButton.onClick.RemoveAllListeners();
        }

        private void OnClickPlay()
        {
            _audioService.StopMusic();
            _audioService.PlaySound(SoundType.ButtonClick);
            _sceneLoader.Load("LevelScene", () => _audioService.PlayMusic());
        }

        private void OnClickSettings()
        {
            _audioService.PlaySound(SoundType.ButtonClick);
            Close(() => _uiPanelService.Open(PanelType.Settings));
        }
    }
}