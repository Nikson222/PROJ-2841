using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.MyEditorCustoms;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Zenject;
using _Scripts._Infrastructure.Services;
using _Scripts._Infrastructure.UI.Base;

namespace _Scripts._Infrastructure.UI
{
    public class SettingsPanel : AnimatedPanel
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Slider _soundSlider;
        [SerializeField] private Slider _musicSlider;

        private AudioService _audioService;
        private UIPanelService _uiPanelService;
        private GamePauseService _gamePauseService;

        private float _lastSoundVolume = 0.5f;
        private float _lastMusicVolume = 0.5f;
        private const float VolumeThreshold = 0.1f;

        [Scene] [SerializeField] private string LevelSceneName;

        [Inject]
        public void Construct(
            AudioService audioService,
            UIPanelService uiPanelService,
            GamePauseService gamePauseService)
        {
            _audioService = audioService;
            _uiPanelService = uiPanelService;
            _gamePauseService = gamePauseService;
        }

        private void OnEnable()
        {
            _soundSlider.onValueChanged.AddListener(SetSound);
            _musicSlider.onValueChanged.AddListener(SetMusic);
            _closeButton.onClick.AddListener(OnClose);

            _soundSlider.value = _audioService.SoundVolume;
            _musicSlider.value = _audioService.MusicVolume;

            _lastSoundVolume = _audioService.SoundVolume;
            _lastMusicVolume = _audioService.MusicVolume;
        }

        private void OnDisable()
        {
            _soundSlider.onValueChanged.RemoveListener(SetSound);
            _musicSlider.onValueChanged.RemoveListener(SetMusic);
            _closeButton.onClick.RemoveListener(OnClose);
        }

        public override void Open()
        {
            if (IsGameplayScene())
                _gamePauseService.RequestPause();

            base.Open();
        }

        public override void Close(System.Action onClosed = null)
        {
            _audioService.PlaySound(SoundType.ButtonClick);

            if (IsGameplayScene())
                _gamePauseService.ReleasePause();

            base.Close(onClosed);
        }

        private void OnClose()
        {
            if (IsGameplayScene())
            {
                _uiPanelService.Close(PanelType.Settings);
            }
            else
            {
                _uiPanelService.Close(PanelType.Settings);
                _uiPanelService.Open(PanelType.Menu);
            }
        }

        private bool IsGameplayScene()
        {
            return SceneManager.GetActiveScene().name == LevelSceneName;
        }

        private void SetSound(float volume)
        {
            if (Mathf.Abs(volume - _lastSoundVolume) >= VolumeThreshold)
            {
                _audioService.PlaySound(SoundType.Slider);
                _lastSoundVolume = volume;
            }

            _audioService.SoundVolume = volume;
        }

        private void SetMusic(float volume)
        {
            if (Mathf.Abs(volume - _lastMusicVolume) >= VolumeThreshold)
            {
                _audioService.PlaySound(SoundType.Slider);
                _lastMusicVolume = volume;
            }

            _audioService.MusicVolume = volume;
        }
    }
}
