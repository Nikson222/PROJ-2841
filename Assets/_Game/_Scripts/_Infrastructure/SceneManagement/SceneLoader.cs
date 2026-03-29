using System;
using System.Collections;
using _Scripts._Infrastructure.Services;
using LoadingCurtain;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts._Infrastructure.SceneManagement
{
    public class SceneLoader
    {
        private readonly CoroutineRunner _coroutineRunner;
        private readonly Curtain _curtain;
        
        public SceneLoader(CoroutineRunner coroutineRunner, Curtain curtain)
        {
            _coroutineRunner = coroutineRunner;
            _curtain = curtain;
        }

        public void Load(string sceneName, Action onLoaded = null, float fakeDelay = 1f)
        {
            _curtain.Open(() => StartLoadScene(sceneName, onLoaded), fakeDelay);
        }

        private void StartLoadScene(string sceneName, Action onLoaded = null)
        {
            _coroutineRunner.StartCoroutine(LoadScene(sceneName, onLoaded, null));
        }

        private IEnumerator LoadScene(string sceneName, Action onLoaded = null, Action onCloseCurtain = null)
        {
            AsyncOperation waitLoadScene = SceneManager.LoadSceneAsync(sceneName);
            
            while (!waitLoadScene.isDone)
                yield return null;
            
            onLoaded?.Invoke();
            
            _curtain.Close(onCloseCurtain);
        }
    }
}