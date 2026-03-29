using System;
using DG.Tweening;
using UnityEngine;

namespace _Scripts._Infrastructure.UI.Base
{
    public abstract class AnimatedPanel : MonoBehaviour, IPanel
    {
        [SerializeField] protected RectTransform _rectTransform;
        [SerializeField] protected float animationTime = 0.15f;
        [SerializeField] protected bool _useUnscaledTime;

        protected Vector3 _initialScale;

        protected virtual void Awake()
        {
            _initialScale = _rectTransform.localScale;
        }

        public virtual void Open()
        {
            gameObject.SetActive(true);
            _rectTransform.localScale = Vector3.zero;
            _rectTransform
                .DOScale(_initialScale, animationTime)
                .SetEase(Ease.OutBack)
                .SetUpdate(_useUnscaledTime);
        }

        public virtual void Close(Action onClosed = null)
        {
            _rectTransform
                .DOScale(Vector3.zero, animationTime)
                .SetEase(Ease.InBack)
                .SetUpdate(_useUnscaledTime)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    onClosed?.Invoke();
                });
        }
    }
}