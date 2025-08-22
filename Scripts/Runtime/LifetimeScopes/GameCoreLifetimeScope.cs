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
        [SerializeField] private AssetLabelReference _configsAssetLabel;

        protected override void RegisterConfigs()
        {
            var configs = Addressables.LoadAssetsAsync<Config>(_configsAssetLabel, null)
                .WaitForCompletion().ToList();
            foreach (var config in configs)
            {
                Register(config);
            }
        }
        
        protected override void RegisterFactories()
        {
            // Register<ViewModelFactory>();
            // Register<ViewsFactory>();
            // Register<ScreensFactory>();
            // Register<HandlersFactory>();
        }

        protected override void RegisterServices()
        {
            // Register<AssetsLoaderService>();
            // Register<ScreensService>();
            // Register<ScenesService>();
        }
    }
}