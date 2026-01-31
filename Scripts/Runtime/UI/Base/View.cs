using System;
using GameCore.Factories;
using VContainer;
using Behaviour = Game.Creatures.Behaviour;

namespace GameCore.UI
{
    public abstract class View<TViewModel> : View
        where TViewModel : ViewModel, new()
    {
        [Inject] private ViewModelFactory _viewModelFactory;

        private Binder[] _viewBinders;
        public TViewModel ViewModel { get; private set; }

        public override void CreateViewModel()
        {
            ViewModel = _viewModelFactory.Create<TViewModel>();
        }
        
        protected void Build(params Binder[] viewBinders)
        {
            foreach (var viewBinder in viewBinders)
            {
                viewBinder.Initialize();
            }
            
            ViewModel.Initialize();
        }

        public override void Dispose()
        {
            foreach (var viewBinder in _viewBinders)
            {
                viewBinder.Dispose();
            }
        }
    }

    public abstract class View : Behaviour, IDisposable
    {
        public abstract void Initialize();
        public abstract void CreateViewModel();
        
        public virtual void Open()
        {
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }

        public virtual void Dispose()
        {
            
        }
    }
}