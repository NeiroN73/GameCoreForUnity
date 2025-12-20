using GameCore.Factories;
using GameCore.Services;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameCore.LifetimeScopes
{
    public class CoreLifetimeScope : BaseLifetimeScope
    {
        [SerializeField] private AssetLabelReference _coreConfigsAssetLabel;

        protected override void RegisterConfigs()
        {
            RegisterConfigs(_coreConfigsAssetLabel);
        }
        
        protected override void RegisterSystems()
        {
            Register<ViewModelFactory>();
            Register<ViewsFactory>();
            Register<ScreensFactory>();
            Register<CreaturesFactory>();
            
            Register<AssetsLoaderService>();
            Register<TickService>();
            Register<ScreensService>();
            Register<ScenesService>();
        }
    }
}