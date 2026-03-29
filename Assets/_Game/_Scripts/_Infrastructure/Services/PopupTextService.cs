using UnityEngine;
using Zenject;

namespace _Scripts._Infrastructure.Services
{
    public class PopupTextService
    {
        private readonly PopupText.Pool _textPool;
        private readonly RectTransform _canvasTransform;

        public PopupTextService(PopupText.Pool pool, RuntimeRefs refs)
        {
            _textPool = pool;
            _canvasTransform = refs.PopupCanvas;
        }

        public void ShowPopupAs(string message, RectTransform asObject, Color color, float showDuration = 1.0f,
            float hideDelay = 0.0f, float hideDuration = 0.5f)
        {
            PopupText popupText = SpawnText();
            CopyRectTransform(asObject, popupText.RectTransform);
            popupText.Setup(message, color, popupText.RectTransform.position, Vector3.one);
            popupText.Show(showDuration, hideDelay, hideDuration, () => _textPool.Despawn(popupText));
        }

        public void ShowPopupOnPosition(string message, Vector3 position, Color color, Vector3 scale,
            float showDuration = 1.0f, float hideDelay = 0.0f, float hideDuration = 0.5f)
        {
            PopupText popupText = SpawnText();
            popupText.Setup(message, color, position, scale);
            popupText.Show(showDuration, hideDelay, hideDuration, () => _textPool.Despawn(popupText));
        }

        private PopupText SpawnText()
        {
            PopupText popupText = _textPool.Spawn();
            popupText.gameObject.transform.SetParent(_canvasTransform, false);
            return popupText;
        }

        private void CopyRectTransform(RectTransform source, RectTransform target)
        {
            target.position = source.position;
            target.rotation = source.rotation;
            target.localScale = source.localScale;
            target.anchorMin = source.anchorMin;
            target.anchorMax = source.anchorMax;
            target.anchoredPosition = source.anchoredPosition;
            target.sizeDelta = source.sizeDelta;
            target.pivot = source.pivot;
        }
    }
}
