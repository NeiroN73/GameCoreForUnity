using GameCore.Handlers;
using UnityEngine;
using VContainer;

namespace GameCore.Factories
{
    public class HandlersFactory : Factory
    {
        [Inject] private IObjectResolver _objectResolver;

        public TEntity Create<TEntity>(TEntity prefab, Vector3 position = default, Quaternion rotation = default) 
            where TEntity : Handler
        {
            var handler = Object.Instantiate(prefab, position, rotation);
            _objectResolver.Inject(handler);
            return handler;
        }
    }
}