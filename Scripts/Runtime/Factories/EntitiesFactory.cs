using Content.Scripts.Entities.Base;
using UnityEngine;
using VContainer;

namespace Content.Scripts.Factories
{
    public class EntitiesFactory : Factory
    {
        [Inject] private IObjectResolver _objectResolver;

        public TEntity Create<TEntity>(TEntity prefab, Vector3 position = default, Quaternion rotation = default) 
            where TEntity : Entity
        {
            var entity = Object.Instantiate(prefab, position, rotation);
            _objectResolver.Inject(entity);
            return entity;
        }
    }
}