using System.Collections.Generic;
using UnityEngine;

namespace DesignPattern
{
    public class ObjectPool
    {
        private Stack<PooledObject> m_stack;
        private PooledObject m_targetPrefab;
        private GameObject m_poolObject;

        public ObjectPool(Transform parent, PooledObject targetPrefab, int initSize = 4)
            => Init(parent, targetPrefab, initSize);


        private void Init(Transform parent, PooledObject targetPrefab, int initSize)
        {
            m_stack = new Stack<PooledObject>();
            m_targetPrefab = targetPrefab;
            m_poolObject = new GameObject($"{m_targetPrefab.name}");
            m_poolObject.transform.parent = parent;

            for(int i = 0; i < initSize; i++)
            {
                CreatePooledObject();
            }
        }

        public PooledObject PopPool()
        {
            if ( m_stack.Count == 0 ) CreatePooledObject();

            PooledObject pooledObject = m_stack.Pop();
            pooledObject.gameObject.SetActive(true);
            return pooledObject;
        }

        public void PushPool(PooledObject target)
        {
            target.transform.parent = m_poolObject.transform;
            target.gameObject.SetActive(false);
            m_stack.Push(target);
        }

        private void CreatePooledObject()
        {
            PooledObject obj = MonoBehaviour.Instantiate(m_targetPrefab);
            obj.PoolInit(this);
            PushPool(obj);
        }
    }
}
