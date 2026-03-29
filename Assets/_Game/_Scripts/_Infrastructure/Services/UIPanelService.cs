using System;
using System.Collections.Generic;
using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.UI.Base;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using static UnityEngine.Object;

namespace _Scripts._Infrastructure.Services
{
    public class UIPanelService : IInitializable, IDisposable
    {
        private readonly DiContainer _container;
        private readonly Transform _canvasTransform;
        private readonly UIPanelConfig _config;

        private readonly Dictionary<PanelType, GameObject> _prefabs = new Dictionary<PanelType, GameObject>();
        private readonly Dictionary<PanelType, IPanel> _instances = new Dictionary<PanelType, IPanel>();
        private readonly List<GameObject> _instantiatedObjects = new List<GameObject>();

        public event Action<PanelType> OnPanelOpened;
        public event Action<PanelType> OnPanelClosed;

        public UIPanelService(DiContainer container, Canvas canvas, UIPanelConfig config)
        {
            _container = container;
            _canvasTransform = canvas.transform;
            _config = config;
        }

        public void Initialize()
        {
            InitializeForScene(SceneManager.GetActiveScene().name);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void Open(PanelType type)
        {
            if (!_instances.TryGetValue(type, out IPanel panel))
                return;

            panel.Open();
            OnPanelOpened?.Invoke(type);
        }

        public void Close(PanelType type)
        {
            if (!_instances.TryGetValue(type, out IPanel panel))
                return;

            panel.Close(() => OnPanelClosed?.Invoke(type));
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Cleanup();
            InitializeForScene(scene.name);
        }

        private void InitializeForScene(string sceneName)
        {
            if (_config == null || _config.PanelEntries == null)
                return;

            for (int i = 0; i < _config.PanelEntries.Count; i++)
            {
                UIPanelConfig.PanelEntry entry = _config.PanelEntries[i];

                if (entry == null)
                    continue;

                if (!entry.SceneName.Equals(sceneName, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (entry.PanelPrefab == null)
                    continue;

                GameObject go = _container.InstantiatePrefab(entry.PanelPrefab, _canvasTransform);
                _container.InjectGameObject(go);

                IPanel panel = go.GetComponentInChildren<IPanel>(true);
                if (panel == null)
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"UIPanelService: prefab '{entry.PanelPrefab.name}' for PanelType '{entry.PanelType}' does not contain component implementing IPanel.");
#endif
                    Destroy(go);
                    continue;
                }

                GameObject panelObject = ((Component)panel).gameObject;

                _instances[entry.PanelType] = panel;
                _prefabs[entry.PanelType] = entry.PanelPrefab;
                _instantiatedObjects.Add(go);

                if (entry.IsInitiallyOpen)
                {
                    panelObject.SetActive(true);
                    panel.Open();
                    OnPanelOpened?.Invoke(entry.PanelType);
                }
                else
                {
                    panelObject.SetActive(false);
                }
            }
        }

        private void Cleanup()
        {
            for (int i = 0; i < _instantiatedObjects.Count; i++)
            {
                GameObject go = _instantiatedObjects[i];
                if (go != null)
                    Destroy(go);
            }

            _instantiatedObjects.Clear();
            _instances.Clear();
            _prefabs.Clear();
        }

        public void Dispose()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Cleanup();
        }
    }
}
