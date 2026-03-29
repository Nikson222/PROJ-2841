using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Scripts._Infrastructure
{
    public class PopupText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private RectTransform _rectTransform;
        
        public RectTransform RectTransform => _rectTransform;
        
        private Pool _textPool;

        public void Setup(string message, Color color, Vector3 position, Vector3 scale)
        {
            _text.text = message;
            _text.color = color;
            transform.position = position;
            transform.localScale = Vector3.zero;
        }

        public void Show(float showDuration, float hideDelay, float hideDuration, Action onComplete)
        {
            transform.DOKill();

            transform
                .DOScale(Vector3.one, showDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    Hide(hideDelay, hideDuration, onComplete);
                });
        }

        private void Hide(float delay, float duration, Action onComplete)
        {
            transform
                .DOScale(Vector3.zero, duration)
                .SetEase(Ease.InBack)
                .SetDelay(delay)
                .OnComplete(() => onComplete?.Invoke());
        }

        public class Pool : MonoMemoryPool<PopupText>
        {
            protected override void OnDespawned(PopupText item)
            {
                item.transform.localScale = Vector3.zero;
                item._text.text = string.Empty;
                base.OnDespawned(item);
            }
        }
    }
}