using System.Linq;
using Content.Scripts.Configs;
using Content.Scripts.Services;
using Content.Scripts.UI.Base;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Content.Scripts.Factories
{
    public class ScreensFactory : Factory, IInitializable
    {
        [Inject] private ViewsFactory _viewsFactory;
        [Inject] private ScreensConfig _screensConfig;
        
        private Transform _parent;

        public void Initialize()
        {
            _parent = Object.Instantiate(_screensConfig.Root, null).transform;
        }

        public async UniTask<TView> CreateAsync<TView>() where TView : View
        {
            var data = _screensConfig.Screens.
                FirstOrDefault(d => d.Type == typeof(TView));
            var handle = await data.Asset.LoadAssetAsync<GameObject>();
            var prefab = handle.GetComponent<TView>();
            var screen = _viewsFactory.Create(prefab, _parent);
            screen.gameObject.SetActive(false);
            return screen;
        }
        
        public TView CreateSync<TView>() where TView : View
        {
            var data = _screensConfig.Screens.
                FirstOrDefault(d => d.Type == typeof(TView));
            var handle = data.Asset.LoadAssetAsync<GameObject>();
            var obj = handle.WaitForCompletion();
            var prefab = obj.GetComponent<TView>();
            var screen = _viewsFactory.Create(prefab, _parent);
            screen.gameObject.SetActive(false);
            return screen;
        }
    }
}
