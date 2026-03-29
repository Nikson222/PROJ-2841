using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts._Infrastructure.Configs
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Configs/AudioConfig", order = 1)]
    public class AudioConfig : ScriptableObject
    {
        public List<SoundConfig> Sounds;
        public List<AudioClip> MusicClips;
        
        public float StartMusicVolume;
        public float StartSoundVolume;
    }

    [Serializable]
    public class SoundConfig
    {
        public SoundType Type;
        public AudioClip Clip;
    }

    public enum SoundType
    {
        ButtonClick,
        Pickup1,
        Pickup2,
        GameOver,
        Slider,
        Swap
    }
}