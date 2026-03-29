using UnityEngine;

namespace _Scripts._Infrastructure
{
    public class RuntimeRefs
    {
        private Canvas _uiCanvas;
        private RectTransform _popupCanvas;
        private AudioSource _soundSource;
        private AudioSource _musicSource;

        public Canvas UICanvas => _uiCanvas;
        public RectTransform PopupCanvas => _popupCanvas;
        public AudioSource SoundSource => _soundSource;
        public AudioSource MusicSource => _musicSource;

        public void SetUICanvas(Canvas canvas)
        {
            _uiCanvas = canvas;
        }

        public void SetPopupCanvas(RectTransform rect)
        {
            _popupCanvas = rect;
        }

        public void SetAudioSources(AudioSource sound, AudioSource music)
        {
            _soundSource = sound;
            _musicSource = music;
        }
    }
}