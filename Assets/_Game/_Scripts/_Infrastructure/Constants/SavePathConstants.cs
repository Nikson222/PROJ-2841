using UnityEngine;

namespace _Scripts._Infrastructure.Constants
{
    public static class SavePathConstants
    {
        public static readonly string AudioSavePath = Application.persistentDataPath + "/audio.json";
        public static readonly string BalanceDataPath = Application.persistentDataPath + "/balance.json";
    }
}