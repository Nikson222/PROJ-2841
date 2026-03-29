using System;
using System.Collections.Generic;
using _Game._Scripts.Game.Models;
using UnityEngine;

namespace _Game._Scripts.Game.Configs
{
    [CreateAssetMenu(fileName = "ChickenVisualConfig", menuName = "Configs/ChickenVisualConfig")]
    public class VisualConfig : ScriptableObject
    {
        [Serializable]
        public class ChickenSpriteEntry
        {
            public ChickenColor Color;
            public Sprite Sprite;
        }

        [SerializeField] private List<ChickenSpriteEntry> _entries = new List<ChickenSpriteEntry>();

        private readonly List<ChickenColor> _availableColorsBuffer = new List<ChickenColor>();

        public Sprite GetSprite(ChickenColor color)
        {
            if (_entries == null)
                return null;

            for (int i = 0; i < _entries.Count; i++)
            {
                ChickenSpriteEntry entry = _entries[i];
                if (entry == null)
                    continue;

                if (entry.Color != color)
                    continue;

                if (entry.Sprite == null)
                    return null;

                return entry.Sprite;
            }

            return null;
        }

        public IReadOnlyList<ChickenColor> GetAvailableColors()
        {
            _availableColorsBuffer.Clear();

            if (_entries == null)
                return _availableColorsBuffer;

            for (int i = 0; i < _entries.Count; i++)
            {
                ChickenSpriteEntry entry = _entries[i];
                if (entry == null)
                    continue;

                if (entry.Sprite == null)
                    continue;

                bool alreadyAdded = false;
                for (int j = 0; j < _availableColorsBuffer.Count; j++)
                {
                    if (_availableColorsBuffer[j] == entry.Color)
                    {
                        alreadyAdded = true;
                        break;
                    }
                }

                if (!alreadyAdded)
                    _availableColorsBuffer.Add(entry.Color);
            }

#if UNITY_EDITOR
            Debug.Log($"ChickenVisualConfig '{name}': available colors count = {_availableColorsBuffer.Count}");
#endif

            return _availableColorsBuffer;
        }
    }
}
