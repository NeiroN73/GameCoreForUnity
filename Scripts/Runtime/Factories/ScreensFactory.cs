using Cysharp.Threading.Tasks;
using GameCore.Configs;
using GameCore.Services;
using GameCore.UI;
using System.Linq;
using GameCore.Utils;
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
            var screenData = GetScreenData<TView>();
            var handle = await _assetsLoaderService.LoadAssetAsync<TView>(screenData.Asset);
            var screen = CreateScreenFromPrefab(handle.GetComponent<TView>());
            return screen;
        }
        
        public TView CreateSync<TView>() where TView : View
        {
            TryCreateRootUI();
            var screenData = GetScreenData<TView>();
            var prefab = _assetsLoaderService.LoadAssetSync<TView>(screenData.Asset);
            var screen = CreateScreenFromPrefab(prefab);
            return screen;
        }
        
        private AddressablePrefabByType<View> GetScreenData<TView>() where TView : View
        {
            var screenData = _screensConfig.Screens
                .FirstOrDefault(d => d.Type == typeof(TView));
            
            return screenData;
        }
        
        private TView CreateScreenFromPrefab<TView>(TView prefab) where TView : View
        {
            var screen = _viewsFactory.Create(prefab, _rootUI.transform);
            screen.gameObject.SetActive(false);
            return screen;
        }
        
        private void TryCreateRootUI()
        {
            if (_rootUI != null)
                return;
            
            _rootUI = new GameObject("RootUI");
            _rootUI.transform.SetParent(null);
        }
    }
}