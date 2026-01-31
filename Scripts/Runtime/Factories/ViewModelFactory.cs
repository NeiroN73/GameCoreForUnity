using GameCore.UI;
using VContainer;

namespace GameCore.Factories
{
    public class ViewModelFactory : Factory
    {
        [Inject] private IObjectResolver _objectResolver;
        
        public T Create<T>() where T : ViewModel, new()
        {
            var viewModel = new T();
            _objectResolver.Inject(viewModel);
            return viewModel;
        }
    }
}
