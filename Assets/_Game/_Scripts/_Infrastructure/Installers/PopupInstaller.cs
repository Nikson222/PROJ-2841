using _Scripts._Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace _Scripts._Infrastructure.Installers
{
    public class PopupInstaller : MonoInstaller
    {
        [SerializeField] private PopupText _popupTextPrefab;

        public override void InstallBindings()
        {
            RectTransform popupCanvas = CreateCanvas();
            RegisterCanvas(popupCanvas);
            BindPool(popupCanvas);
            BindService();
        }

        private RectTransform CreateCanvas()
        {
            GameObject go = new("PopupCanvas");

            Canvas canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            UnityEngine.UI.CanvasScaler scaler = go.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;

            if (go.GetComponent<RectTransform>() == null)
                go.AddComponent<RectTransform>();

            DontDestroyOnLoad(go);
            return go.GetComponent<RectTransform>();
        }

        private void RegisterCanvas(RectTransform rect)
        {
            RuntimeRefs refs = GetOrBindRuntimeRefs();
            refs.SetPopupCanvas(rect);
        }

        private void BindPool(RectTransform under)
        {
            Container
                .BindMemoryPool<PopupText, PopupText.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(_popupTextPrefab)
                .UnderTransform(under);
        }

        private void BindService()
        {
            Container
                .BindInterfacesAndSelfTo<PopupTextService>()
                .AsSingle();
        }

        private RuntimeRefs GetOrBindRuntimeRefs()
        {
            if (Container.HasBinding<RuntimeRefs>())
                return Container.Resolve<RuntimeRefs>();

            RuntimeRefs created = new();
            Container.Bind<RuntimeRefs>().FromInstance(created).AsSingle();
            return created;
        }
    }
}
