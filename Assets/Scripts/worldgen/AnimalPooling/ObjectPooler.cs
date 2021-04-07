using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class ObjectPooler : MonoBehaviour
{
    /// <summary>
    /// A Pool has a tag for the contained element, rabbit. A prefab and an amount of that object to start with (size)
    /// </summary>
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
    private bool isInstantiated = false;

    /// <summary>
    /// Instantiate this object and make a dictionary for the queues.
    /// </summary>
    private void Awake()
    {
        instance = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
    }
    
    /// <summary>
    /// Instantiate all pools. 
    /// </summary>
    void Start()
    {
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    /// <summary>
    /// Called when the terrain generator is finished placing all animals and object. 
    /// </summary>
    public void HandleFinishedSpawning()
    {
        foreach (Pool pool in pools)
        {
            string objTag = pool.tag;
            for (int i = poolDictionary[objTag].Count; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                poolDictionary[objTag].Enqueue(obj);
            }
        }

        isInstantiated = true;
    }

    /// <summary>
    /// When the terrain generator makes a new animal, the pool handles the animals start method (onObjectSpawn)
    /// and subscribes to the animal mating and death. 
    /// </summary>
    /// <param name="objectToSpawn"> The pooled object the terrain generator will spawn. </param>
    /// <param name="tag"> Tag of the animal, must match names in terrain generator. </param>
    public void HandleAnimalInstantiated(GameObject objectToSpawn, string tag)
    {
        if (!isInstantiated)
        {
            if (poolDictionary != null && poolDictionary.ContainsKey(tag))
            {
                objectToSpawn.SetActive(true);
                objectToSpawn.GetComponent<IPooledObject>()?.onObjectSpawn();

                objectToSpawn.GetComponent<AnimalController>().Dead += HandleDeadAnimal;
                objectToSpawn.GetComponent<AnimalController>().SpawnNew += HandleBirthAnimal;

                poolDictionary[tag].Enqueue(objectToSpawn);
            }
        }
    }

    /// <summary>
    /// Make more space in the queue and disable the animal.
    /// </summary>
    /// <param name="animalController"> Controller of the deceased animal. </param>
    private void HandleDeadAnimal(AnimalController animalController)
    {
        GameObject animalObj;
        (animalObj = animalController.gameObject).SetActive(false);
        
        switch (animalController.animalModel)
        {
            case RabbitModel _:
                poolDictionary["Rabbits"].Enqueue(animalObj);
                break;
            case WolfModel _:
                poolDictionary["Wolfs"].Enqueue(animalObj);
                break;
            case DeerModel _:
                poolDictionary["Deers"].Enqueue(animalObj);
                break;
            case BearModel _:
                poolDictionary["Bears"].Enqueue(animalObj);
                break;
        }
    }

    /// <summary>
    /// Handles animal birth by instantiating the animal as previously using the pool
    /// </summary>
    /// <param name="childModel"> Model to spawn animal with </param>
    /// <param name="pos"> Where the animal is to be spawned </param>
    /// <param name="energy"> The energy of the child </param>
    /// <param name="hydration"> The hydration of the child </param>
    private void HandleBirthAnimal(AnimalModel childModel, Vector3 pos, float energy, float hydration)
    {
        GameObject child = null;
        switch (childModel)
        {
            case RabbitModel _:
                child = SpawnFromPool("Rabbits", pos, Quaternion.identity);
                break;
            case WolfModel _:
                child = SpawnFromPool("Wolfs", pos, Quaternion.identity);
                break;
            case DeerModel _:
                child = SpawnFromPool("Bears", pos, Quaternion.identity);
                break;
            case BearModel _:
                child = SpawnFromPool("Deers", pos, Quaternion.identity);
                break;
        }

        if (child != null)
        {
            AnimalController childController = child.GetComponent<AnimalController>();
            childController.animalModel = childModel;
            childController.animalModel.currentEnergy = energy;
            childController.animalModel.currentHydration = hydration;

            // update the childs speed (in case of mutation).
            childController.animalModel.traits.maxSpeed = 1;
        }
    }

    /// <summary>
    /// Activates an object from the given pool named "tag". 
    /// </summary>
    /// <param name="tag"> Name of the pool to spawn from, ie Rabbits</param>
    /// <param name="position"> Where the object is to be spawned </param>
    /// <param name="rotation"> Rotation of the object</param>
    /// <returns></returns>
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
                    for (int i = 0; i < 20; i++)
                    {
                        GameObject obj = Instantiate(pool.prefab);
                        obj.SetActive(false);
                        poolDictionary[tag].Enqueue(obj);
                    }
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
            
            objectToSpawn.GetComponent<AnimalController>().Dead += HandleDeadAnimal;
            objectToSpawn.GetComponent<AnimalController>().SpawnNew += HandleBirthAnimal;

            return objectToSpawn;
        }

        return null;
    }
}