using System;
using System.Collections.Generic;
using _Game._Scripts.Game.Models;
using UnityEngine;

namespace _Game._Scripts.Game.Configs
{
    [CreateAssetMenu(fileName = "ChickenHighlightConfig", menuName = "Configs/ChickenHighlightConfig")]
    public class HighlightConfig : ScriptableObject
    {
        [Serializable]
        public class HighlightEntry
        {
            public ChickenColor Color;
            public Color HighlightColor;
        }

        [SerializeField] private List<HighlightEntry> _entries = new List<HighlightEntry>();

        public Color GetColor(ChickenColor color)
        {
            if (_entries == null)
                return Color.white;

            for (int i = 0; i < _entries.Count; i++)
            {
                HighlightEntry entry = _entries[i];
                if (entry == null)
                    continue;

                if (entry.Color == color)
                    return entry.HighlightColor;
            }

            return Color.white;
        }
    }
}