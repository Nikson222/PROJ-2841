using UnityEngine;

namespace _Game._Scripts.Game.Configs
{
    [CreateAssetMenu(fileName = "ChickenGameConfig", menuName = "Configs/ChickenGameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("Очки")]
        [Min(1)] public int BaseScorePerChicken = 10;
        [Min(0)] public int BonusPerExtraChicken = 5;

        [Header("Раннер")]
        [Min(0.1f)] public float RunnerTravelTimeSeconds = 6.0f;

        [Tooltip("Доп. отступ между курицами в ширинах курицы: 0 = вплотную, 0.5 = половина ширины и т.д.")]
        [Min(0f)] public float RunnerSpacingOffset = 0.0f;
    }
}