using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LoadingCurtain
{
    public class Curtain : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Text _loadingText;
        [SerializeField] private float _dotUpdateTime = 0.5f;

        private Coroutine _loadingDotsCoroutine;
        private bool _isAnimatingDots = false;

        private Coroutine _loadingCoroutine;
        
        void Awake()
        {
            _animator = GetComponent<Animator>();

            if(_loadingCoroutine == null)
                Close();

            DontDestroyOnLoad(transform.parent);
        }

        public void Open(Action onLoaded = null, float fakeDelay = 1f)
        {
            if (_loadingCoroutine != null)
                StopCoroutine(_loadingCoroutine);
            
            _loadingCoroutine = StartCoroutine(LoadingRoutine(true, onLoaded, fakeDelay));
        }

        public void Close(Action onLoaded = null)
        {
            if (_loadingCoroutine != null)
                StopCoroutine(_loadingCoroutine);
            
            _loadingCoroutine = StartCoroutine(LoadingRoutine(false, onLoaded));
        }

        private IEnumerator LoadingRoutine(bool isOpen, Action onLoaded = null, float fakeDelay = 1f)
        {
            _animator.SetBool("IsLoading", isOpen);
            _animator.SetBool("IsCurtainProcess", true);

            if (isOpen)
            {
                _isAnimatingDots = true;
                _loadingDotsCoroutine = StartCoroutine(AnimateLoadingDots());
            }

            yield return new WaitForSeconds(fakeDelay);

            _animator.SetBool("IsCurtainProcess", false);

            if (_loadingDotsCoroutine != null && !isOpen)
            {
                _isAnimatingDots = false;
                StopCoroutine(_loadingDotsCoroutine);
                _loadingText.text = "LOADING";
            }

            onLoaded?.Invoke();
        }

        private IEnumerator AnimateLoadingDots()
        {
            string baseText = "LOADING";
            int dotCount = 0;
            int dotTargetCount = 3;

            while (_isAnimatingDots)
            {
                _loadingText.text = baseText + new string('.', dotCount);

                dotCount = (dotCount + 1) % (dotTargetCount + 1);

                yield return new WaitForSeconds(_dotUpdateTime);
            }
        }
    }
}