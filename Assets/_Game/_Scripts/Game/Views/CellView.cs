using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Game._Scripts.Game.Views
{
    public class CellView : MonoBehaviour
    {
        [SerializeField] private Image _background;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _highlight;

        private RectTransform _rectTransform;
        private Vector3 _iconDefaultScale;

        private const float PopDuration = 0.25f;
        private const float ShrinkDuration = 0.25f;

        public RectTransform RectTransform => _rectTransform;

        private void Awake()
        {
            _rectTransform = (RectTransform)transform;

            if (_highlight != null)
                _highlight.enabled = false;

            if (_icon != null)
            {
                _iconDefaultScale = _icon.rectTransform.localScale;
                _icon.enabled = false;
            }
        }

        public void ShowSprite(Sprite sprite, bool withPop)
        {
            if (_icon == null)
                return;

            _icon.rectTransform.DOKill();

            _icon.sprite = sprite;
            bool hasSprite = sprite != null;
            _icon.enabled = hasSprite;

            if (!hasSprite || !withPop)
                return;

            _icon.rectTransform.localScale = Vector3.zero;
            _icon.rectTransform
                .DOScale(_iconDefaultScale, PopDuration)
                .SetEase(Ease.OutBack);
        }

        public void ShowSpriteFromWorld(Sprite sprite, Vector3 fromWorldPosition, float duration)
        {
            if (_icon == null)
                return;

            _icon.rectTransform.DOKill();

            if (sprite == null)
            {
                HideSprite(false);
                return;
            }

            _icon.sprite = sprite;
            _icon.enabled = true;

            Vector3 targetPos = _icon.rectTransform.position;

            _icon.rectTransform.position = fromWorldPosition;
            _icon.rectTransform.localScale = _iconDefaultScale;

            _icon.rectTransform
                .DOMove(targetPos, duration)
                .SetEase(Ease.OutQuad);
        }

        public void HideSprite(bool withShrink)
        {
            if (_icon == null)
                return;

            _icon.rectTransform.DOKill();

            if (!withShrink)
            {
                _icon.sprite = null;
                _icon.enabled = false;
                _icon.rectTransform.localScale = _iconDefaultScale;
                _icon.rectTransform.position = _rectTransform.position;
                return;
            }

            _icon.rectTransform
                .DOScale(0f, ShrinkDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    _icon.sprite = null;
                    _icon.enabled = false;
                    _icon.rectTransform.localScale = _iconDefaultScale;
                    _icon.rectTransform.position = _rectTransform.position;
                });
        }

        public void SetHighlight(bool value)
        {
            if (_highlight == null)
                return;

            _highlight.enabled = value;
        }

        public void SetHighlight(bool value, Color color)
        {
            if (_highlight == null)
                return;

            _highlight.color = color;
            _highlight.enabled = value;
        }
    }
}
