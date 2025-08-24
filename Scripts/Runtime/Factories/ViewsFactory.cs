using GameCore.UI;
using UnityEngine;
using VContainer;

namespace GameCore.Factories
{
    public class ViewsFactory : Factory
    {
        [Inject] private IObjectResolver _objectResolver;
        
        public TView Create<TView>(TView prefab, Transform parent)
            where TView : View
        {
            var view = Object.Instantiate(prefab, parent);
            InitializeView(view);
            return view;
        }

        public void InitializeView(View view)
        {
            _objectResolver.Inject(view);
            view.Initialize();
        }
    }
}