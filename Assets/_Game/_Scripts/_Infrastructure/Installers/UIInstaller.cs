using System;
using _Scripts._Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace _Scripts._Infrastructure.Installers
{
    public class UIInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _canvasPrefab;
        [SerializeField] private Configs.UIPanelConfig _uiPanelConfig;

        public override void InstallBindings()
        {
            Canvas canvas = CreateCanvas();
            BindCanvas(canvas);
            BindUIPanelService(canvas, _uiPanelConfig);
        }

        private Canvas CreateCanvas()
        {
            Canvas canvas = Container.InstantiatePrefabForComponent<Canvas>(_canvasPrefab);
            DontDestroyOnLoad(canvas.gameObject);
            return canvas;
        }

        private void BindCanvas(Canvas canvas)
        {
            RuntimeRefs refs = GetOrBindRuntimeRefs();
            refs.SetUICanvas(canvas);
        }

        private void BindUIPanelService(Canvas canvas, Configs.UIPanelConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            Container
                .Bind<Configs.UIPanelConfig>()
                .FromInstance(config)
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<UIPanelService>()
                .AsSingle()
                .WithArguments(canvas, config);

            Container.InjectGameObject(canvas.gameObject);
        }

        private RuntimeRefs GetOrBindRuntimeRefs()
        {
            if (Container.HasBinding<RuntimeRefs>())
                return Container.Resolve<RuntimeRefs>();

            RuntimeRefs created = new RuntimeRefs();
            Container.Bind<RuntimeRefs>().FromInstance(created).AsSingle();
            return created;
        }
    }
}
