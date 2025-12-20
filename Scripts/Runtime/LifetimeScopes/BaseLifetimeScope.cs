using System.Linq;
using GameCore.Configs;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;

namespace GameCore.LifetimeScopes
{
    public abstract class BaseLifetimeScope : LifetimeScope
    {
        protected IContainerBuilder Builder;
        
        protected override void Configure(IContainerBuilder builder)
        {
            Builder = builder;

            RegisterConfigs();
            RegisterSystems();
        }
        
        protected void Register<T>() where T : class
        {
            Builder.Register<T>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }
        
        protected void Register<T>(T instance) where T : class
        {
            Builder.RegisterInstance(instance).AsImplementedInterfaces().AsSelf();
        }
        
        protected  virtual void RegisterConfigs()
        {
        }
        
        protected virtual void RegisterSystems()
        {
        }

        protected void RegisterConfigs(AssetLabelReference assetLabelReference)
        {
            var configs = Addressables.LoadAssetsAsync<Config>(assetLabelReference, null)
                .WaitForCompletion().ToList();
            foreach (var config in configs)
            {
                Register(config);
            }
        }
    }
}