using System;
using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.Data;
using Core.Infrastructure.SaveLoad;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts._Infrastructure.Services
{
    public class AudioService : ISavable, Zenject.IInitializable
    {
        private readonly AudioSource _soundSource;
        private readonly AudioSource _musicSource;
        private readonly AudioConfig _config;

        private float _savedMusicVolume;
        private float _savedSoundVolume;
        private float _lastMusicVolume;

        public float SoundVolume
        {
            get => _savedSoundVolume;
            set
            {
                _soundSource.volume = value;
                _savedSoundVolume = value;
            }
        }

        public float MusicVolume
        {
            get
            {
                return _savedMusicVolume;
            }
            set
            {
                _musicSource.volume = value;
                _savedMusicVolume = value;
            }
        }

        public Type DataType => typeof(AudioData);
        public string SavePath => Constants.SavePathConstants.AudioSavePath;

        public AudioService(RuntimeRefs refs, AudioConfig config)
        {
            _soundSource = refs.SoundSource;
            _musicSource = refs.MusicSource;
            _config = config;
        }

        public void Initialize()
        {
            if (_lastMusicVolume == 0)
                _lastMusicVolume = _musicSource.volume;
        }

        public void PlaySound(SoundType type)
        {
            System.Collections.Generic.List<SoundConfig> clips = _config.Sounds.FindAll(x => x.Type == type);
            if (clips.Count == 0)
                return;

            AudioClip clip = clips[Random.Range(0, clips.Count)].Clip;
            _soundSource.PlayOneShot(clip);
        }

        public void PlayMusic(bool withFade = true)
        {
            if (_config.MusicClips.Count == 0)
                return;

            AudioClip clip = _config.MusicClips[Random.Range(0, _config.MusicClips.Count)];
            _musicSource.clip = clip;
            _musicSource.Play();

            if (withFade)
            {
                _musicSource.volume = 0f;
                DOTween
                    .To(() => _musicSource.volume, x => _musicSource.volume = x, _savedMusicVolume, 1f)
                    .SetEase(Ease.InOutSine);
            }
        }

        public void StopMusic(bool withFade = true)
        {
            if (!withFade)
            {
                _musicSource.Stop();
                return;
            }

            DOTween
                .To(() => _musicSource.volume, x => _musicSource.volume = x, 0f, 1f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    _musicSource.Stop();
                    _musicSource.volume = _savedMusicVolume;
                });
        }

        public void MuteMusic()
        {
            _musicSource.mute = true;
        }

        public void UnMuteMusic()
        {
            _musicSource.mute = false;
        }

        public void MuteSound()
        {
            _soundSource.mute = true;
        }

        public void UnMuteSound()
        {
            _soundSource.mute = false;
        }

        public object GetData()
        {
            if (_musicSource == null || _soundSource == null)
                return new AudioData(_savedMusicVolume, _savedSoundVolume);

            return new AudioData(_musicSource.volume, _soundSource.volume);
        }

        public void SetData(object data)
        {
            if (data is not AudioData audioData)
                return;

            MusicVolume = audioData.MusicVolume;
            SoundVolume = audioData.SoundVolume;
        }

        public void SetInitialData()
        {
            MusicVolume = _config.StartMusicVolume;
            SoundVolume = _config.StartSoundVolume;
        }
    }
}
