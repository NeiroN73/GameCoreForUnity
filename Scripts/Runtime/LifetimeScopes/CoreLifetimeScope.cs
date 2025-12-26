using GameCore.Factories;
using GameCore.Services;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameCore.LifetimeScopes
{
    public class CoreLifetimeScope : BaseLifetimeScope
    {
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