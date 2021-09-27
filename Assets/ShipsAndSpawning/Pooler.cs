using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Pooling is very difficult to implement in general. Objects can extend IOnPoolAndRetrieve to make the process easier, but
 * in general, recycling GameObjects can be a design pattern nightmare with filled with race conditions and checks that
 * need to happen. Use sparingly — so far, the only things that have been pooled.
 * 
 * Note: GameObjects to be pooled require a PoolableGameObjectLink to function.
 * 
 * More info in IOnPoolAndRetrieve
 */

public class Pooler : MonoBehaviour
{

    public static Pooler Instance;

    public Dictionary<PooledObjectType, ObjectPool> PooledObjectsLists = new Dictionary<PooledObjectType, ObjectPool>();

    void Start()
    {
        Instance = this;
        Debug.Log("Started pooler");

        foreach (PooledObjectType pooledObjectType in Enum.GetValues(typeof(PooledObjectType)))
        {
            PooledObjectsLists.Add(pooledObjectType, new ObjectPool(pooledObjectType));
        }
    }

    public void PoolGameObject(PoolableGameObjectLink toPool, bool doExecuteOnPool = true, bool setAsInactive = true)
    {
       
        if (doExecuteOnPool)
        {
            IOnPoolAndRetrieve[] poolables = toPool.gameObject.GetComponentsInChildren<IOnPoolAndRetrieve>();

            for(int j = 0; j < poolables.Length; j++)
            {
                poolables[j].OnPool();
            }

        }
        if (setAsInactive)
            toPool.gameObject.SetActive(false);

        PooledObjectsLists[toPool.PooledObjectType].Pool.Enqueue(toPool.gameObject);
    }

    public GameObject GetPooledGameObject(PooledObjectType type, bool doExecuteOnRetrieve = true)
    {
        if (PooledObjectsLists[type].Pool.Count == 0) return null;

        GameObject pooledObject = PooledObjectsLists[type].Pool.Dequeue();

        if (doExecuteOnRetrieve)
        {
            IOnPoolAndRetrieve[] poolablesToExecuteOnRetrieve = pooledObject.gameObject.GetComponentsInChildren<IOnPoolAndRetrieve>();

            for (int j = 0; j < poolablesToExecuteOnRetrieve.Length; j++)
            {
                poolablesToExecuteOnRetrieve[j].OnRetrieve();
            }
        }

        return pooledObject;
    }
    
}

public class ObjectPool
{
    public PooledObjectType PoolObjectType;

    public Queue<GameObject> Pool = new Queue<GameObject>();

    public ObjectPool(PooledObjectType type)
    {
        PoolObjectType = type;
    }
}