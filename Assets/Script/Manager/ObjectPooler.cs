using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

namespace ObjectPool
{
    public class ObjectPooler : MonoBehaviour
    {
        private Stack<PooledObject> pool;

        [SerializeField] private PooledObject pooledObjectPrefab;
        [SerializeField] private int count;
        [SerializeField] private bool enableCreation;

        private void Awake()
        {
            pool =new Stack<PooledObject>();
        }
        private void Start()
        {
            CreatePool(pooledObjectPrefab, count);
        }
        public void CreatePool(PooledObject prefab, int count)
        {
            for(int index = 0; index < count; ++index)
            {
                PooledObject instance = Instantiate(prefab);
                instance.gameObject.SetActive(false);
                instance.returnPool = this;
                pool.Push(instance);
            }
        }

        public PooledObject Get(Vector3 position, Quaternion rotation)
        {
            if (pool.Count > 0)
            {
                PooledObject instance = pool.Pop();
                instance.gameObject.SetActive(true);
                instance.transform.position = position;
                instance.transform.rotation = rotation;
                return instance;
            }
            else if(enableCreation)
            {
                PooledObject instance = Instantiate(pooledObjectPrefab);
                instance.gameObject.SetActive(true);
                instance.transform.position = position;
                instance.transform.rotation = rotation;
                instance.returnPool = this;
                return instance;
            }
            else
            {
                return null;
            }
        }
        public void ReturnToPool(PooledObject instance)
        {
            instance.gameObject.SetActive(false);
            pool.Push(instance);
        }
    }
}
