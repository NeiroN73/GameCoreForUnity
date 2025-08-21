using UnityEngine;
using UnityEngine.Pool;

namespace Content.Scripts.Entities.Base
{
    public class EntityPool<TEntity> where TEntity : PooledEntity
    {
        private readonly ObjectPool<TEntity> pool;
        private readonly TEntity prefab;
        private readonly Transform parent;

        public EntityPool(TEntity prefab, Transform parent = null, int defaultCapacity = 1, int maxSize = 10000)
        {
            this.prefab = prefab;
            this.parent = parent;

            pool = new ObjectPool<TEntity>(
                createFunc: CreateEntity,
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: OnDestroy,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
        }

        private TEntity CreateEntity()
        {
            TEntity entity = Object.Instantiate(prefab, parent);
            return entity;
        }

        private void OnGet(TEntity entity)
        {
            entity.gameObject.SetActive(true);
            entity.OnGet();
        }

        private void OnRelease(TEntity entity)
        {
            entity.gameObject.SetActive(false);
            entity.OnReturn();
        }

        private void OnDestroy(TEntity entity)
        {
            Object.Destroy(entity.gameObject);
        }

        public TEntity Get() => pool.Get();

        public void Release(TEntity entity) => pool.Release(entity);

        public void Clear() => pool.Clear();
    }
}