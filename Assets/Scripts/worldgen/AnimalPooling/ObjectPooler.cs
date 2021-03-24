using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [Serializable]
    public class  Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    
    #region Singleton
    public static ObjectPooler instance;
    #endregion
    
    private void Awake()
    {
        instance = this;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    { 
        if (poolDictionary != null && poolDictionary.ContainsKey(tag))
        {
            GameObject objectToSpawn = poolDictionary[tag].Dequeue();
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            
            objectToSpawn.GetComponent<IPooledObject>()?.onObjectSpawn();

            poolDictionary[tag].Enqueue(objectToSpawn);
            return objectToSpawn;
        }
        return null;
    }
}
