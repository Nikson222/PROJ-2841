using UnityEngine;

namespace _Game._Scripts.Game.Configs
{
    [CreateAssetMenu(fileName = "ChickenBoardConfig", menuName = "Configs/ChickenBoardConfig")]
    public class BoardConfig : ScriptableObject
    {
        [Min(1)] public int Columns = 5;
        [Min(1)] public int Rows = 10;
        [Min(3)] public int MinGroupSize = 3;

        [Min(0f), Tooltip("Отступ между курицами (используется как spacing в GridLayoutGroup).")]
        public float ChickenSpacing = 0f;

        [Tooltip("Индекс строки, заполняемой до Game Over (0 — нижняя).")]
        public int DeadLineRow = 8;
    }
}