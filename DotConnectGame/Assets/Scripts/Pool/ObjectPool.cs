using System;
using System.Collections;
using System.Collections.Generic;
using Pool;
using UnityEngine;

public abstract class ObjectPool<T> : MonoBehaviour,IObjectPool<T> where T :MonoBehaviour, IPoolableObject<T>
{
    public T poolObjectPrefab;
    public int startSize;


    protected Queue<T> queue = new Queue<T>();
    protected Queue<T> activeQueue = new Queue<T>();

    protected virtual void Awake()
    {
        
        for (int i = 0; i < startSize; i++)
        {
            CreatePoolObject();
        }
    }

    
    protected virtual void CreatePoolObject()
    {
        T poolObject = Instantiate(poolObjectPrefab,transform);
        poolObjectPrefab.PoolParent = this;
        Enqueue(poolObject);
    }
    public void Enqueue(T pooledObject)
    {
        EnqueueSettings(pooledObject);
        queue.Enqueue(pooledObject);
    }

    public T Dequeue()
    {
        if (queue.Count == 0)
        {
            CreatePoolObject();
            return Dequeue();
        }

        T poolObject = queue.Dequeue();
        activeQueue.Enqueue(poolObject);
        DequeueSettings(poolObject);
        return poolObject;
    }

    public virtual void ResetPool()
    {
      
    }
    protected abstract void EnqueueSettings(T poolObject);
    protected abstract void DequeueSettings(T poolObject);
}
