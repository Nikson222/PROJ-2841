using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.MyEditorCustoms;
using _Scripts._Infrastructure.SceneManagement;
using _Scripts._Infrastructure.Services;
using _Scripts._Infrastructure.UI.Base;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts._Infrastructure.UI
{
    public class PausePanel : AnimatedPanel
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _menuButton;
        [Scene] [SerializeField] private string _menuSceneName;

        private SceneLoader _sceneLoader;
        private AudioService _audioService;
        private UIPanelService _uiPanelService;
        private GamePauseService _gamePauseService;

        [Inject]
        public void Construct(
            SceneLoader sceneLoader,
            AudioService audioService,
            UIPanelService uiPanelService,
            GamePauseService gamePauseService)
        {
            _sceneLoader = sceneLoader;
            _audioService = audioService;
            _uiPanelService = uiPanelService;
            _gamePauseService = gamePauseService;
        }

        private void OnEnable()
        {
            if (_continueButton != null)
                _continueButton.onClick.AddListener(OnClickContinue);

            if (_menuButton != null)
                _menuButton.onClick.AddListener(OnClickMenu);
        }

        private void OnDisable()
        {
            if (_continueButton != null)
                _continueButton.onClick.RemoveListener(OnClickContinue);

            if (_menuButton != null)
                _menuButton.onClick.RemoveListener(OnClickMenu);
        }

        public override void Open()
        {
            _gamePauseService.RequestPause();
            base.Open();
        }

        public override void Close(System.Action onClosed = null)
        {
            _gamePauseService.ReleasePause();
            base.Close(onClosed);
        }

        private void OnClickContinue()
        {
            _audioService.PlaySound(SoundType.ButtonClick);
            _uiPanelService.Close(PanelType.Pause);
        }

        private void OnClickMenu()
        {
            _audioService.PlaySound(SoundType.ButtonClick);
            _audioService.StopMusic();

            _uiPanelService.Close(PanelType.Pause);

            _sceneLoader.Load(_menuSceneName, () =>
            {
                _audioService.PlayMusic();
                _gamePauseService.ForceResume();
            });
        }
    }
}
