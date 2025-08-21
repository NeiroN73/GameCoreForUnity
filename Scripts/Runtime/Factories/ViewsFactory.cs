using Content.Scripts.UI.Base;
using UnityEngine;
using VContainer;

namespace Content.Scripts.Factories
{
    public class ViewsFactory : Factory
    {
        [Inject] private IObjectResolver _objectResolver;
        
        public TView Create<TView>(TView prefab, Transform parent)
            where TView : View
        {
            var view = Object.Instantiate(prefab, parent);
            _objectResolver.Inject(view);
            view.Initialize();
            return view;
        }
    }
}