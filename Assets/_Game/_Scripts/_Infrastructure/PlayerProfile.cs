using System;
using _Scripts._Infrastructure.Constants;
using _Scripts.Data;
using Core.Infrastructure.SaveLoad;

namespace _Scripts._Infrastructure
{
    public class PlayerProfile : ISavable
    {
        private int _currency;
        private int _maxScore;
        
        public int Currency => _currency;
        public int MaxScore => _maxScore;

        public string SavePath => SavePathConstants.BalanceDataPath;
        public Type DataType { get; } = typeof(PlayerProfileData);

        public event Action<int> OnCurrencyChanged;

        public void SetMaxScore(int scoreViewScore)
        {
            if(MaxScore < scoreViewScore)
                _maxScore = scoreViewScore;
        }

        public void AddCurrency(int count)
        {
            _currency += count;

            OnCurrencyChanged?.Invoke(_currency);
        }

        public bool RemoveCurrency(int count)
        {
            if (_currency < count)
                return false;

            _currency -= count;

            OnCurrencyChanged?.Invoke(_currency);
            return true;
        }

        public object GetData()
        {
            var balanceData = new PlayerProfileData(Currency, MaxScore);

            return balanceData;
        }

        public void SetData(object data)
        {
            if (data is not PlayerProfileData balanceData) return;
            
            _currency = balanceData.Currency;
            _maxScore = balanceData.MaxScore;
        }

        public void SetInitialData()
        {
            _currency = 0;
            _maxScore = 0;
        }
    }
}