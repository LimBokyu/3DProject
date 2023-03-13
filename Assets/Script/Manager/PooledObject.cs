using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectPool
{
    public class PooledObject : MonoBehaviour
    {
        [SerializeField] private float returnTime;

        public ObjectPooler returnPool { get; set; }

        private void OnEnable()
        {
            StartCoroutine(DelayToReturn());
        }

        private IEnumerator DelayToReturn()
        {
            yield return new WaitForSeconds(returnTime);
            returnPool.ReturnToPool(this);
        }
    }
}
