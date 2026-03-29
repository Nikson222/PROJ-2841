using System;
using _Game._Scripts.Game.Configs;
using _Game._Scripts.Game.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace _Game._Scripts.Game.Views
{
    public class CurrentView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image _icon;
        [SerializeField] private RectTransform _dragArea;

        private VisualConfig _visualConfig;
        private RectTransform _rectTransform;

        private int _columns;
        private Vector2 _startAnchoredPosition;
        private bool _isDragging;

        public event Action<int, bool> OnDragOverColumn;
        public event Action<int, bool> OnDropOnColumn;

        public bool IsDragging => _isDragging;

        [Inject]
        public void Construct(VisualConfig visualConfig)
        {
            _visualConfig = visualConfig;
        }

        private void Awake()
        {
            _rectTransform = (RectTransform)transform;
            _startAnchoredPosition = _rectTransform.anchoredPosition;
            HideChicken();
        }

        public void Initialize(int columns)
        {
            _columns = columns;
        }

        public void ShowChicken(ChickenColor color)
        {
            Sprite sprite = _visualConfig.GetSprite(color);
            _icon.sprite = sprite;
            _icon.enabled = sprite != null;
        }

        public void HideChicken()
        {
            _icon.enabled = false;
            _icon.sprite = null;
        }

        public void ResetPosition()
        {
            _rectTransform.anchoredPosition = _startAnchoredPosition;
        }

        public void SetAutoProgress(float t)
        {
            if (_dragArea == null || _columns <= 0)
                return;

            if (_isDragging)
                return;

            t = Mathf.Clamp01(t);

            Rect areaRect = _dragArea.rect;

            float x = Mathf.Lerp(areaRect.xMin, areaRect.xMax, t);
            float y = Mathf.Lerp(areaRect.yMin, areaRect.yMin + areaRect.height * 0.4f, 0.5f);

            Vector2 local = new Vector2(x, y);
            Vector2 world = _dragArea.TransformPoint(local);
            _rectTransform.position = world;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_dragArea == null || _columns <= 0)
                return;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _dragArea, eventData.position, eventData.pressEventCamera, out localPoint);

            Rect areaRect = _dragArea.rect;

            float clampedX = Mathf.Clamp(localPoint.x, areaRect.xMin, areaRect.xMax);
            float clampedY = Mathf.Clamp(localPoint.y, areaRect.yMin, areaRect.yMax);

            Vector2 local = new Vector2(clampedX, clampedY);
            Vector2 world = _dragArea.TransformPoint(local);
            _rectTransform.position = world;

            int columnIndex = GetColumnIndexFromLocalX(clampedX, areaRect);
            bool isUpperHalf = clampedY >= areaRect.center.y;

            OnDragOverColumn?.Invoke(columnIndex, isUpperHalf);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;

            if (_dragArea == null || _columns <= 0)
            {
                OnDropOnColumn?.Invoke(-1, false);
                ResetPosition();
                return;
            }

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _dragArea, eventData.position, eventData.pressEventCamera, out localPoint);

            Rect areaRect = _dragArea.rect;

            float clampedX = Mathf.Clamp(localPoint.x, areaRect.xMin, areaRect.xMax);
            float clampedY = Mathf.Clamp(localPoint.y, areaRect.yMin, areaRect.yMax);

            int columnIndex = GetColumnIndexFromLocalX(clampedX, areaRect);
            bool isUpperHalf = clampedY >= areaRect.center.y;

            OnDropOnColumn?.Invoke(columnIndex, isUpperHalf);
        }

        private int GetColumnIndexFromLocalX(float localX, Rect areaRect)
        {
            if (_columns <= 0)
                return -1;

            float width = areaRect.width;

            if (width <= 0.0f)
                return -1;

            float normalized = (localX - areaRect.xMin) / width;
            float scaled = normalized * _columns;
            int index = Mathf.FloorToInt(scaled);

            if (index < 0 || index >= _columns)
                return -1;

            return index;
        }
    }
}
