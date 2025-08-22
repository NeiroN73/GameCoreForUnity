using System.Linq;
using Cysharp.Threading.Tasks;
using GameCore.Configs;
using GameCore.Services;
using GameCore.UI;
using UnityEngine;
using VContainer;

namespace GameCore.Factories
{
    public class ScreensFactory : Factory
    {
        [Inject] private ViewsFactory _viewsFactory;
        [Inject] private ScreensConfig _screensConfig;
        [Inject] private AssetsLoaderService _assetsLoaderService;
        
        private GameObject _rootUI;
        
        public async UniTask<TView> CreateAsync<TView>() where TView : View
        {
            TryCreateRootUI();
            var data = _screensConfig.Screens.
                FirstOrDefault(d => d.Type == typeof(TView));
            var handle = await _assetsLoaderService.LoadAssetAsync<TView>(data.Asset);
            var prefab = handle.GetComponent<TView>();
            var screen = _viewsFactory.Create(prefab, _rootUI.transform);
            screen.gameObject.SetActive(false);
            return screen;
        }
        
        public TView CreateSync<TView>() where TView : View
        {
            TryCreateRootUI();
            var data = _screensConfig.Screens.
                FirstOrDefault(d => d.Type == typeof(TView));
            var prefab = _assetsLoaderService.LoadAssetSync<TView>(data.Asset);
            var screen = _viewsFactory.Create(prefab, _rootUI.transform);
            screen.gameObject.SetActive(false);
            return screen;
        }
        
        private void TryCreateRootUI()
        {
            if(_rootUI)
                return;
            
            _rootUI = new GameObject("RootUI");
            _rootUI.transform.SetParent(null);
        }
    }
}
