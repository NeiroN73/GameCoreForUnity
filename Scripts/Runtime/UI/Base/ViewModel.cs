using System;
using System.Collections.Generic;
using System.Linq;
using R3;

namespace Content.Scripts.UI
{
    public abstract class ViewModel : IDisposable
    {
        private Dictionary<string, ViewModelBinder> _viewModelBinders = new();
        public IReadOnlyDictionary<string, ViewModelBinder> ViewModelBinders => _viewModelBinders;
        protected CompositeDisposable Disposable = new();

        protected void Bind(params ViewModelBinder[] binders)
        {
            _viewModelBinders = binders.ToDictionary(b => b.Id);
        }

        public abstract void Initialize();

        public virtual void Dispose()
        {
            Disposable.Dispose();
        }
    }
}