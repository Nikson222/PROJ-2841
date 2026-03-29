using System;
using _Game._Scripts.Game.Configs;
using _Game._Scripts.Game.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Game._Scripts.Game.Views
{
    public class RunnerItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image _icon;

        private RectTransform _rectTransform;
        private RectTransform _panelRect;
        private RectTransform _trackRect;
        private int _columns;

        private float _progress;
        private bool _isDragging;

        public int Id { get; private set; }
        public bool IsDragging => _isDragging;
        public float Progress => _progress;

        public RectTransform RectTransform => _rectTransform;

        public event Action<RunnerItemView> OnBeginDragRequested;
        public event Action<RunnerItemView> OnEndDragRequested;
        public event Action<RunnerItemView, Vector3, bool> OnDragOver;
        public event Action<RunnerItemView, Vector3, bool> OnDrop;

        private void Awake()
        {
            _rectTransform = (RectTransform)transform;
        }

        public void Initialize(
            int id,
            ChickenColor color,
            RectTransform panelRect,
            RectTransform trackRect,
            int columns,
            VisualConfig visualConfig)
        {
            Id = id;
            _panelRect = panelRect;
            _trackRect = trackRect != null ? trackRect : panelRect;
            _columns = columns;

            Sprite sprite = null;

            if (visualConfig != null)
                sprite = visualConfig.GetSprite(color);

            if (sprite == null)
            {
                _icon.sprite = null;
                _icon.enabled = false;
            }
            else
            {
                _icon.sprite = sprite;
                _icon.enabled = true;
            }

            AdaptScaleToTrackHeight();

            _progress = 0f;
            _isDragging = false;
        }

        public void SetProgress(float progress)
        {
            _progress = progress;

            if (_isDragging)
                return;

            UpdatePositionByProgress();
        }

        public void ResetToProgress()
        {
            UpdatePositionByProgress();
        }

        private void UpdatePositionByProgress()
        {
            if (_trackRect == null)
                return;

            Rect rect = _trackRect.rect;

            float x = Mathf.Lerp(rect.xMin, rect.xMax, _progress);
            float y = rect.center.y;

            Vector2 local = new Vector2(x, y);
            Vector3 world = _trackRect.TransformPoint(local);
            _rectTransform.position = world;
        }

        private void AdaptScaleToTrackHeight()
        {
            if (_trackRect == null)
                return;

            float trackHeight = _trackRect.rect.height;
            if (trackHeight <= 0f)
                return;

            float currentHeight = _rectTransform.rect.height;
            if (currentHeight <= 0f)
                return;

            float targetHeight = trackHeight * 0.8f;
            float scale = targetHeight / currentHeight;

            if (scale <= 0f)
                return;

            Vector3 localScale = _rectTransform.localScale;
            localScale.x *= scale;
            localScale.y *= scale;
            _rectTransform.localScale = localScale;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;
            OnBeginDragRequested?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_panelRect == null || _columns <= 0)
                return;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _panelRect, eventData.position, eventData.pressEventCamera, out localPoint);

            Rect rect = _panelRect.rect;

            float clampedX = Mathf.Clamp(localPoint.x, rect.xMin, rect.xMax);
            float clampedY = Mathf.Clamp(localPoint.y, rect.yMin, rect.yMax);

            Vector2 local = new Vector2(clampedX, clampedY);
            Vector3 world = _panelRect.TransformPoint(local);
            _rectTransform.position = world;

            float normalized = (clampedX - rect.xMin) / rect.width;
            if (normalized < 0f)
                normalized = 0f;
            else if (normalized > 1f)
                normalized = 1f;

            _progress = normalized;

            bool isUpperHalf = clampedY >= rect.center.y;

            OnDragOver?.Invoke(this, world, isUpperHalf);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;

            if (_panelRect == null || _columns <= 0)
            {
                OnDrop?.Invoke(this, _rectTransform.position, false);
                ResetToProgress();
                OnEndDragRequested?.Invoke(this);
                return;
            }

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _panelRect, eventData.position, eventData.pressEventCamera, out localPoint);

            Rect rect = _panelRect.rect;

            float clampedX = Mathf.Clamp(localPoint.x, rect.xMin, rect.xMax);
            float clampedY = Mathf.Clamp(localPoint.y, rect.yMin, rect.yMax);

            Vector2 local = new Vector2(clampedX, clampedY);
            Vector3 world = _panelRect.TransformPoint(local);

            bool isUpperHalf = clampedY >= rect.center.y;

            OnDrop?.Invoke(this, world, isUpperHalf);
            OnEndDragRequested?.Invoke(this);
        }
    }
}
