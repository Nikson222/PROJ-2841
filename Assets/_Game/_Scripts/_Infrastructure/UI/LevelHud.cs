using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.Services;
using _Scripts._Infrastructure.UI.Base;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts._Infrastructure.UI
{
    public class LevelHud : MonoBehaviour
    {
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _pauseButton;

        private UIPanelService _uiPanelService;
        private AudioService _audioService;

        private int _blockingPanelsCount;

        [Inject]
        public void Construct(UIPanelService uiPanelService, AudioService audioService)
        {
            _uiPanelService = uiPanelService;
            _audioService = audioService;
        }

        private void OnEnable()
        {
            if (_settingsButton != null)
                _settingsButton.onClick.AddListener(OnClickSettings);

            if (_pauseButton != null)
                _pauseButton.onClick.AddListener(OnClickPause);

            if (_uiPanelService != null)
            {
                _uiPanelService.OnPanelOpened += HandlePanelOpened;
                _uiPanelService.OnPanelClosed += HandlePanelClosed;
            }
        }

        private void OnDisable()
        {
            if (_settingsButton != null)
                _settingsButton.onClick.RemoveListener(OnClickSettings);

            if (_pauseButton != null)
                _pauseButton.onClick.RemoveListener(OnClickPause);

            if (_uiPanelService != null)
            {
                _uiPanelService.OnPanelOpened -= HandlePanelOpened;
                _uiPanelService.OnPanelClosed -= HandlePanelClosed;
            }
        }

        private void OnClickSettings()
        {
            _audioService.PlaySound(SoundType.ButtonClick);
            _uiPanelService.Open(PanelType.Settings);
        }

        private void OnClickPause()
        {
            _audioService.PlaySound(SoundType.ButtonClick);
            _uiPanelService.Open(PanelType.Pause);
        }

        private void HandlePanelOpened(PanelType type)
        {
            if (type != PanelType.Settings && type != PanelType.Pause)
                return;

            _blockingPanelsCount++;
            UpdateButtonsInteractable();
        }

        private void HandlePanelClosed(PanelType type)
        {
            if (type != PanelType.Settings && type != PanelType.Pause)
                return;

            _blockingPanelsCount--;
            if (_blockingPanelsCount < 0)
                _blockingPanelsCount = 0;

            UpdateButtonsInteractable();
        }

        private void UpdateButtonsInteractable()
        {
            bool interactable = _blockingPanelsCount == 0;

            if (_settingsButton != null)
                _settingsButton.interactable = interactable;

            if (_pauseButton != null)
                _pauseButton.interactable = interactable;
        }
    }
}
