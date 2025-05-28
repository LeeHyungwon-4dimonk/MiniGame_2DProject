using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPattern
{
    public class PooledObject : MonoBehaviour
    {
        public ObjectPool ObjPool { get; private set; }

        public void PoolInit(ObjectPool objPool)
        {
            ObjPool = objPool;
        }

        public void ReturnPool()
        {
            ObjPool.PushPool(this);
        }
    }
}
