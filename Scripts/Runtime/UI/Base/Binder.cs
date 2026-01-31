using System;
using R3;

namespace GameCore.UI
{
    public abstract class Binder : IDisposable
    {
        protected readonly CompositeDisposable Disposable = new();
        
        public abstract void Initialize();
        public virtual void Dispose()
        {
            Disposable.Dispose();
        }
    }
}
