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
            RegisterFactories();
            RegisterServices();
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
        
        protected  virtual void RegisterFactories()
        {
        }

        protected virtual void RegisterServices()
        {
        }
    }
}