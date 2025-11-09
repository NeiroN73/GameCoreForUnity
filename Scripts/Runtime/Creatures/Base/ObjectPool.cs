using UnityEngine;

namespace GameCore.Creatures
{
    public class ObjectPool<TObject>
        where TObject : MonoBehaviour, IPoolable
    {
        private readonly UnityEngine.Pool.ObjectPool<TObject> pool;
        private readonly TObject prefab;
        private readonly Transform parent;

        public ObjectPool(TObject prefab, Transform parent = null, int defaultCapacity = 1, int maxSize = 10000)
        {
            this.prefab = prefab;
            this.parent = parent;

            pool = new UnityEngine.Pool.ObjectPool<TObject>(
                createFunc: OnCreate,
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: OnDestroy,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
        }

        private TObject OnCreate()
        {
            TObject handler = Object.Instantiate(prefab, parent);
            return handler;
        }

        private void OnGet(TObject handler)
        {
            handler.gameObject.SetActive(true);
            handler.OnGet();
        }

        private void OnRelease(TObject handler)
        {
            handler.gameObject.SetActive(false);
            handler.OnReturn();
        }

        private void OnDestroy(TObject handler)
        {
            Object.Destroy(handler.gameObject);
        }

        public TObject Get() => pool.Get();

        public void Release(TObject handler) => pool.Release(handler);

        public void Clear() => pool.Clear();
    }
}