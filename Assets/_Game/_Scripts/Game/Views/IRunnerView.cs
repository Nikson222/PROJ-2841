using System;
using _Game._Scripts.Game.Models;
using UnityEngine;

namespace _Game._Scripts.Game.Views
{
    public interface IRunnerView
    {
        event Action<int> OnChickenBeginDrag;
        event Action<int> OnChickenEndDrag;
        event Action<int, Vector3, bool> OnChickenDragOver;
        event Action<int, Vector3, bool, float> OnChickenDropped;

        void Initialize(int columns);
        void SpawnChicken(int id, ChickenColor color, float initialProgress);
        void UpdateChickenProgress(int id, float progress);
        void RemoveChicken(int id);
        void ResetChickenToProgress(int id);

        float GetTrackWidth();
        float GetChickenWidth(int id);
    }
}