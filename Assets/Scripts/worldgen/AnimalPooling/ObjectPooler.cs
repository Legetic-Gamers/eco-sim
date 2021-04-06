using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectPooler : MonoBehaviour
{
    [Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    #region Singleton

    public static ObjectPooler instance;

    #endregion

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        instance = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
    }
    void Start()
    {
        FindObjectOfType<AnimalSpawner>().onAnimalInstantiated += HandleAnimalInstantiated;
        
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

    private void HandleAnimalInstantiated(GameObject objectToSpawn, string tag)
    {
        if (poolDictionary != null && poolDictionary.ContainsKey(tag))
        {
            poolDictionary[tag].Dequeue();
            
            objectToSpawn.GetComponent<IPooledObject>()?.onObjectSpawn();
            
            Debug.Log("Handling");
            
            if (objectToSpawn.CompareTag("Animal"))
            {
                objectToSpawn.GetComponent<AnimalController>().Dead += HandleDeadAnimal;
                objectToSpawn.GetComponent<AnimalController>().SpawnNew += HandleBirthAnimal;
            }
            
            objectToSpawn.SetActive(true);
        }
    }

    private void HandleDeadAnimal(AnimalController animalController)
    {
        GameObject animalObj;
        (animalObj = animalController.gameObject).SetActive(false);
        
        switch (animalController.animalModel)
        {
            case RabbitModel _:
                poolDictionary["Rabbit"].Enqueue(animalObj);
                break;
            case WolfModel _:
                poolDictionary["Wolf"].Enqueue(animalObj);
                break;
            case DeerModel _:
                poolDictionary["Deer"].Enqueue(animalObj);
                break;
            case BearModel _:
                poolDictionary["Bear"].Enqueue(animalObj);
                break;
        }
    }
    
    private void HandleBirthAnimal(AnimalModel childModel, Vector3 pos, float energy, float hydration)
    {
        GameObject child = SpawnFromPool("Rabbit", pos, Quaternion.identity);
        
        AnimalController childController = child.GetComponent<AnimalController>();
        childController.animalModel = childModel;
        childController.animalModel.currentEnergy = energy;
        childController.animalModel.currentHydration = hydration;

        // update the childs speed (in case of mutation).
        childController.animalModel.traits.maxSpeed = 1;
        
        //Debug.Log(childController.animalModel.generation);
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary[tag].Any())
        {
            // Empty queue, make more
            foreach (var pool in pools)
            {
                if (pool.tag.Equals(tag))
                {
                    pool.size += 20;
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    poolDictionary[tag].Enqueue(obj);
                }
            }
        }

        if (poolDictionary != null && poolDictionary.ContainsKey(tag))
        {
                GameObject objectToSpawn = poolDictionary[tag].Dequeue();

                objectToSpawn.transform.position = position;
                objectToSpawn.transform.rotation = rotation;
                objectToSpawn.SetActive(true);

                //TODO Maintain list of all components for more performance
                objectToSpawn.GetComponent<IPooledObject>()?.onObjectSpawn();

                if (objectToSpawn.CompareTag("Animal"))
                {
                    objectToSpawn.GetComponent<AnimalController>().Dead += HandleDeadAnimal;
                    objectToSpawn.GetComponent<AnimalController>().SpawnNew += HandleBirthAnimal;
                }

                return objectToSpawn;
        }

        return null;
    }
}