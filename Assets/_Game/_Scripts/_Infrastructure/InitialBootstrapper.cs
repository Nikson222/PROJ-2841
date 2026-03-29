using System;
using _Scripts._Infrastructure.MyEditorCustoms;
using _Scripts._Infrastructure.SceneManagement;
using _Scripts._Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace _Scripts._Infrastructure
{
    public class InitialBootstrapper : MonoBehaviour
    {
        private AudioService _audioService;
        private SceneLoader _sceneLoader;

        [Scene] [SerializeField] private string _menuScene;

        [Inject]
        public void Construct(AudioService audioService, SceneLoader sceneLoader)
        {
            _audioService = audioService;
            _sceneLoader = sceneLoader;
        }

        private void Awake()
        {
            Application.targetFrameRate = 120;
            LoadMenuScene();
        }

        private void LoadMenuScene()
        {
            _sceneLoader.Load(_menuScene, () => _audioService.PlayMusic(), 3f);
            Destroy(gameObject);
        }
    }
}