using Content.Scripts.UI;
using VContainer;

namespace Content.Scripts.Factories
{
    public class ViewModelFactory : Factory
    {
        [Inject] private IObjectResolver _objectResolver;

        public T Create<T>(params ViewBinder[] viewBinders) where T : ViewModel, new()
        {
            var viewModel = new T();
            _objectResolver.Inject(viewModel);
            viewModel.Initialize();
            
            foreach (var viewBinder in viewBinders)
            {
                if (viewModel.ViewModelBinders.TryGetValue(viewBinder.Id, out var viewModelBinder))
                {
                    viewBinder.ViewModelBinder = viewModelBinder;
                }
            }
            
            return viewModel;
        }
    }
}
