using System.Linq;
using GameCore.Configs;
using GameCore.Factories;
using GameCore.Services;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameCore.LifetimeScopes
{
    public class GameCoreLifetimeScope : BaseLifetimeScope
    {
        [SerializeField] private AssetLabelReference _coreConfigsAssetLabel;

        protected override void RegisterConfigs()
        {
            var configs = Addressables.LoadAssetsAsync<Config>(_coreConfigsAssetLabel, null)
                .WaitForCompletion().ToList();
            foreach (var config in configs)
            {
                Register(config);
            }
        }
        
        protected override void RegisterFactories()
        {
            Register<ViewModelFactory>();
            Register<ViewsFactory>();
            Register<ScreensFactory>();
            Register<HandlersFactory>();
        }

        protected override void RegisterServices()
        {
            Register<AssetsLoaderService>();
            Register<TickService>();
            Register<ScreensService>();
            Register<ScenesService>();
        }
    }
}