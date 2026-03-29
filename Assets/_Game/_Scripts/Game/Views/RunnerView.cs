using System;
using System.Collections.Generic;
using _Game._Scripts.Game.Configs;
using _Game._Scripts.Game.Models;
using UnityEngine;
using Zenject;

namespace _Game._Scripts.Game.Views
{
    public class RunnerView : MonoBehaviour, IRunnerView
    {
        [SerializeField] private RectTransform _panelRect;
        [SerializeField] private RectTransform _trackRect;
        [SerializeField] private RunnerItemView _itemPrefab;

        private readonly Dictionary<int, RunnerItemView> _items = new Dictionary<int, RunnerItemView>();

        private int _columns;
        private VisualConfig _visualConfig;

        public event Action<int> OnChickenBeginDrag;
        public event Action<int> OnChickenEndDrag;
        public event Action<int, Vector3, bool> OnChickenDragOver;
        public event Action<int, Vector3, bool, float> OnChickenDropped;

        [Inject]
        public void Construct(VisualConfig visualConfig)
        {
            _visualConfig = visualConfig;
        }

        public void Initialize(int columns)
        {
            _columns = columns;
            ClearAll();
        }

        public void SpawnChicken(int id, ChickenColor color, float initialProgress)
        {
            if (_items.ContainsKey(id))
                return;

            RectTransform parent = _trackRect != null ? _trackRect : _panelRect;

            RunnerItemView item = Instantiate(_itemPrefab, parent);
            item.Initialize(id, color, _panelRect, _trackRect, _columns, _visualConfig);
            item.SetProgress(initialProgress);

            item.OnBeginDragRequested += HandleItemBeginDrag;
            item.OnEndDragRequested += HandleItemEndDrag;
            item.OnDragOver += HandleItemDragOver;
            item.OnDrop += HandleItemDrop;

            _items.Add(id, item);
        }

        public void UpdateChickenProgress(int id, float progress)
        {
            RunnerItemView item;
            if (!_items.TryGetValue(id, out item))
                return;

            item.SetProgress(progress);
        }

        public void RemoveChicken(int id)
        {
            RunnerItemView item;
            if (!_items.TryGetValue(id, out item))
                return;

            item.OnBeginDragRequested -= HandleItemBeginDrag;
            item.OnEndDragRequested -= HandleItemEndDrag;
            item.OnDragOver -= HandleItemDragOver;
            item.OnDrop -= HandleItemDrop;

            _items.Remove(id);
            Destroy(item.gameObject);
        }

        public void ResetChickenToProgress(int id)
        {
            RunnerItemView item;
            if (!_items.TryGetValue(id, out item))
                return;

            item.ResetToProgress();
        }

        public float GetTrackWidth()
        {
            if (_trackRect != null)
                return _trackRect.rect.width;

            if (_panelRect != null)
                return _panelRect.rect.width;

            return 0f;
        }

        public float GetChickenWidth(int id)
        {
            RunnerItemView item;
            if (!_items.TryGetValue(id, out item) || item == null)
                return 0f;

            RectTransform rectTransform = item.RectTransform;
            return rectTransform.rect.width;
        }

        private void HandleItemBeginDrag(RunnerItemView view)
        {
            OnChickenBeginDrag?.Invoke(view.Id);
        }

        private void HandleItemEndDrag(RunnerItemView view)
        {
            OnChickenEndDrag?.Invoke(view.Id);
        }

        private void HandleItemDragOver(RunnerItemView view, Vector3 worldPosition, bool isUpperHalf)
        {
            OnChickenDragOver?.Invoke(view.Id, worldPosition, isUpperHalf);
        }

        private void HandleItemDrop(RunnerItemView view, Vector3 worldPosition, bool isUpperHalf)
        {
            OnChickenDropped?.Invoke(view.Id, worldPosition, isUpperHalf, view.Progress);
        }

        private void ClearAll()
        {
            foreach (KeyValuePair<int, RunnerItemView> pair in _items)
            {
                RunnerItemView item = pair.Value;
                if (item == null)
                    continue;

                item.OnBeginDragRequested -= HandleItemBeginDrag;
                item.OnEndDragRequested -= HandleItemEndDrag;
                item.OnDragOver -= HandleItemDragOver;
                item.OnDrop -= HandleItemDrop;

                Destroy(item.gameObject);
            }

            _items.Clear();
        }

        private void OnDestroy()
        {
            ClearAll();
        }
    }
}
