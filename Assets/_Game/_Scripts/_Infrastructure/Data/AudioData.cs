using _Scripts.Data;

namespace _Scripts._Infrastructure.Data
{
    public class AudioData : SaveData
    {
        public readonly float MusicVolume;
        public readonly float SoundVolume;

        public AudioData(float musicVolume, float soundVolume)
        {
            MusicVolume = musicVolume;
            SoundVolume = soundVolume;
        }
    }
}