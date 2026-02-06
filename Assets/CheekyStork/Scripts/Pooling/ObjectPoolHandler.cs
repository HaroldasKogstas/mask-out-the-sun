using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace CheekyStork.Pooling
{
    /// <summary>
    /// A generic class that allows the pooling of any monobehaviour object that implements the IPoolable interface.
    /// </summary>
    public class ObjectPoolHandler<T> where T : MonoBehaviour, IPoolable<T>
    {
        private ObjectPool<T> _objectPool;

        /// <summary>
        /// Constructs a pool of provided IPoolable objects. PoolCapacity defines the initial memory allocation for the pool.
        /// PreInstantiatePool will immediately instantiate the pool up to poolCapacity (saves on dynamic instantiation calls later).
        /// </summary>
        public ObjectPoolHandler(T prefabToPool, Transform parentTransform, int poolCapacity = 10, bool preInstantiatePool = false)
        {
            CreateObjectPool(prefabToPool, parentTransform, poolCapacity);

            if (preInstantiatePool)
            {
                PreInstantiatePool(poolCapacity);
            }
        }

        /// <summary>
        /// Returns an object from the pool and subscribes to its ReturnToPool() method.
        /// </summary>
        public T Get()
        {
            T pooledObjectInstance = _objectPool.Get();
            pooledObjectInstance.ReturnToPool += CleanupObject;

            return pooledObjectInstance;
        }

        /// <summary>
        /// Overloaded Get method to provide an initial position and rotation.
        /// </summary>
        public T Get(Vector3 position, Quaternion rotation)
        {
            T pooledObjectInstance = Get();

            pooledObjectInstance.transform.SetPositionAndRotation(position, rotation);

            return pooledObjectInstance;
        }

        private void CreateObjectPool(T prefab, Transform parent, int defaultCapacity)
        {
            _objectPool = new ObjectPool<T>(
                createFunc: () => InstantiateObject(prefab, parent),
                actionOnGet: (obj) => obj.gameObject.SetActive(true),
                actionOnRelease: (obj) => obj.gameObject.SetActive(false),
                actionOnDestroy: (obj) => GameObject.Destroy(obj.gameObject),
                collectionCheck: false,
                defaultCapacity: defaultCapacity);
        }

        private T InstantiateObject(T prefab, Transform parent)
        {
            T pooledObjectInstance = GameObject.Instantiate(prefab, parent);
            pooledObjectInstance.gameObject.SetActive(false);

            return pooledObjectInstance;
        }

        private void PreInstantiatePool(int poolCapacity)
        {
            List<T> preinstantiationPool = new();

            for (int i = 0; i < poolCapacity; i++)
            {
                T objectInstance = Get();
                preinstantiationPool.Add(objectInstance);
            }

            foreach (T pooledObject in preinstantiationPool)
            {
                CleanupObject(pooledObject);
            }
        }

        // When the object is returned to the pool, unsubscribe from its ReturnToPool() method and release it back into the pool.
        private void CleanupObject(T pooledObjectInstance)
        {
            pooledObjectInstance.ResetObject();
            pooledObjectInstance.ReturnToPool -= CleanupObject;

            _objectPool.Release(pooledObjectInstance);
        }
    }
}