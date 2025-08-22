using UnityEngine;
using UnityEngine.Pool;

namespace GameCore.Handlers
{
    public class HandlerPool<THandler> where THandler : PooledHandler
    {
        private readonly ObjectPool<THandler> pool;
        private readonly THandler prefab;
        private readonly Transform parent;

        public HandlerPool(THandler prefab, Transform parent = null, int defaultCapacity = 1, int maxSize = 10000)
        {
            this.prefab = prefab;
            this.parent = parent;

            pool = new ObjectPool<THandler>(
                createFunc: OnCreate,
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: OnDestroy,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
        }

        private THandler OnCreate()
        {
            THandler handler = Object.Instantiate(prefab, parent);
            return handler;
        }

        private void OnGet(THandler handler)
        {
            handler.gameObject.SetActive(true);
            handler.OnGet();
        }

        private void OnRelease(THandler handler)
        {
            handler.gameObject.SetActive(false);
            handler.OnReturn();
        }

        private void OnDestroy(THandler handler)
        {
            Object.Destroy(handler.gameObject);
        }

        public THandler Get() => pool.Get();

        public void Release(THandler handler) => pool.Release(handler);

        public void Clear() => pool.Clear();
    }
}