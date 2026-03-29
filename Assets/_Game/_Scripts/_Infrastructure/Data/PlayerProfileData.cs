using System;

namespace _Scripts.Data
{
    [Serializable]
    public class PlayerProfileData : SaveData
    {
        public readonly int Currency;
        public readonly int MaxScore;
        
        public PlayerProfileData(int currency, int maxScore)
        {
            Currency = currency;
            MaxScore = maxScore;
        }
    }
}