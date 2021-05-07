using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DataCollection;
using DefaultNamespace;
using Menus;
using UnityEngine;
using UnityEngine.AI;
using ViewController;

public class ObjectPooler : MonoBehaviour
{
    private GameObject groupObject;

    /// <summary>
    /// A Pool has a label for the contained element, rabbit. A prefab and an amount of that object to start with (size)
    /// </summary>
    [Serializable]
    public class Pool : ISerializationCallbackReceiver
    {
        public string label;
        public GameObject prefab;
        public int size;
        
        public void OnBeforeSerialize()
        {
            if (prefab.TryGetComponent(out IPooledObject pooledObject))
            {
                label = pooledObject.GetObjectLabel();
            }
            
        }

        public void OnAfterDeserialize()
        {
            //do nothing
        }
    }

    public static ObjectPooler Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType(typeof(ObjectPooler)) as ObjectPooler;

            return instance;
        }
        set { instance = value; }
    }

    public static ObjectPooler instance;
    
    public List<Pool> pools;
    public Dictionary<string, ConcurrentQueue<GameObject>> poolDictionary;
    public Dictionary<string, Stack<GameObject>> stackDictionary;
    private bool showCanvasForAll;
    private bool isFinishedPlacing;
    private DataHandler dh;

    private void Awake()
    {
        
        instance = this;
        groupObject = new GameObject("Pool");

        poolDictionary = new Dictionary<string, ConcurrentQueue<GameObject>>();
        stackDictionary = new Dictionary<string, Stack<GameObject>>();
        foreach (Pool pool in pools)
        {
            ConcurrentQueue<GameObject> objectPool = new ConcurrentQueue<GameObject>();
            Stack<GameObject> objectStack = new Stack<GameObject>();
            poolDictionary.Add(pool.label, objectPool);
            stackDictionary.Add(pool.label, objectStack);
        }
        showCanvasForAll = OptionsMenu.alwaysShowParameterUI;
        isFinishedPlacing = false;
    }

    private void Start()
    {
        dh = FindObjectOfType<DataHandler>();
        
        //If there are no object placer
        if (!FindObjectOfType<ObjectPlacement>())
        {
            InitWithoutObjectPlacer();
        }
    }

    public void InitWithoutObjectPlacer()
    {
        AnimalController[] animals = FindObjectsOfType<AnimalController>();
        PlantController[] plants = FindObjectsOfType<PlantController>();
        
        //Debug.Log(animals.Length);
        //Debug.Log(plants.Length);
        
        foreach(AnimalController animalController in animals)
        {
            HandleAnimalInstantiated(animalController.gameObject, animalController.GetObjectLabel());
        }

        foreach (PlantController plant in plants)
        {
            HandleFoodInstantiated(plant.gameObject, plant.GetObjectLabel());
        }
    }
    
    /// <summary>
    /// Called when the terrain generator is finished placing all animals and object. 
    /// </summary>
    public void HandleFinishedSpawning()
    {
        if (!isFinishedPlacing)
        {
            foreach (Pool pool in pools)
            {
                string objlabel = pool.label;
                if (stackDictionary[objlabel].Count > 0)
                {
                    for (int i = stackDictionary[objlabel].Count; i < pool.size; i++)
                    {
                        GameObject obj = Instantiate(pool.prefab, groupObject.transform, true);
                        obj.SetActive(false);
                        poolDictionary[objlabel].Enqueue(obj);
                        
                    }

                    for (int i = 0; i < stackDictionary[objlabel].Count - 1; i++) poolDictionary[objlabel].Enqueue(stackDictionary[objlabel].Pop());
                }
            }
            isFinishedPlacing = true;
        }
    }

    /// <summary>
    /// When the terrain generator makes a new animal, the pool handles the animals start method (onObjectSpawn)
    /// and subscribes to the animal mating and death. 
    /// </summary>
    /// <param name="objectToSpawn"> The pooled object the terrain generator will spawn. </param>
    /// <param name="label"> label of the animal, must match names in terrain generator. </param>
    public void HandleAnimalInstantiated(GameObject objectToSpawn, string label)
    {
        if (poolDictionary != null && poolDictionary.ContainsKey(label))
        {
            objectToSpawn.SetActive(true);
            objectToSpawn.GetComponent<IPooledObject>()?.onObjectSpawn();
            if (objectToSpawn.TryGetComponent(out AnimalController animalController))
            {
                dh.LogNewAnimal(animalController.animalModel);
                animalController.deadState.onDeath += HandleDeadAnimal;
                animalController.SpawnNew += HandleBirthAnimal;
                animalController.GetComponentInChildren<ParameterUI>(true).SetUIActive(showCanvasForAll);
            }

            if (stackDictionary != null && stackDictionary.ContainsKey(label))
            {
                stackDictionary[label].Push(objectToSpawn);
            }
        }
    }

    /// <summary>
    /// Make more space in the queue and disable the animal.
    /// </summary>
    /// <param name="animalController"> Controller of the deceased animal. </param>
    /// <param name="gotEaten"></param>
    public void HandleDeadAnimal(AnimalController animalController, bool gotEaten)
    {
        animalController.deadState.onDeath -= HandleDeadAnimal;
        animalController.SpawnNew -= HandleBirthAnimal;
        if(gotEaten) StartCoroutine(HandleDeadAnimalDelay(animalController, 0f));
        else StartCoroutine(HandleDeadAnimalDelay(animalController, 5f));
    }

    private IEnumerator HandleDeadAnimalDelay(AnimalController animalController, float delay)
    {
        AnimalModel.CauseOfDeath cause;
        AnimalModel am = animalController.animalModel;
        if (am.currentEnergy <= 0) cause = AnimalModel.CauseOfDeath.Energy;
        else if (am.currentHydration <= 0) cause = AnimalModel.CauseOfDeath.Hydration;
        else if (am.age >= am.traits.ageLimit) cause = AnimalModel.CauseOfDeath.Age;
        else if (am.currentHealth <= 0) cause = AnimalModel.CauseOfDeath.Health;
        else cause = AnimalModel.CauseOfDeath.Eaten;

        if (animalController is WolfController)
        {
            Debug.Log("Cause of death: " + cause.ToString());
        }
        
        yield return new WaitForSeconds(delay);
        if (animalController != null)
        {
            if (animalController.TryGetComponent(out IPooledObject pooledObject))
            {
                GameObject animalObj;
                (animalObj = animalController.gameObject).SetActive(false);
            
                dh.LogDeadAnimal(am, cause, (transform.position - animalController.startVector).magnitude);
                
                poolDictionary[pooledObject.GetObjectLabel()].Enqueue(animalObj);    
            } else Debug.Log("IpooledObject does not exist on: " + animalController.name);
        }
    }

    /// <summary>
    /// Handles animal birth by instantiating the animal as previously using the pool
    /// </summary>
    /// <param name="childModel"> Model to spawn animal with </param>
    /// <param name="pos"> Where the animal is to be spawned </param>
    /// <param name="energy"> The energy of the child </param>
    /// <param name="hydration"> The hydration of the child </param>
    /// <param name="isSmart"> If the animals has ML brain </param>
    private void HandleBirthAnimal(AnimalModel childModel, Vector3 pos, float energy, float hydration, string label)
    {
        GameObject child;

        child = SpawnFromPool(label, pos, Quaternion.identity);
        
        if (child != null)
        {
            AnimalController childController = child.GetComponent<AnimalController>();
            childController.animalModel = childModel;
            //Debug.Log(childController.animalModel.generation);
            childController.animalModel.currentEnergy = energy;
            childController.animalModel.currentHydration = hydration;
            childController.GetComponentInChildren<ParameterUI>(true).SetUIActive(showCanvasForAll);
            
            // update the childs speed (in case of mutation).
            childController.animalModel.traits.maxSpeed = 1;
            dh.LogNewAnimal(childModel);
        }
        
        
    }

    /// <summary>
    /// Activates an object from the given pool named "label". 
    /// </summary>
    /// <param name="label"> Name of the pool to spawn from, ie Rabbits</param>
    /// <param name="position"> Where the object is to be spawned </param>
    /// <param name="rotation"> Rotation of the object</param>
    /// <returns></returns>
    public GameObject SpawnFromPool(string label, Vector3 position, Quaternion rotation)
    {

        if (poolDictionary != null && poolDictionary.ContainsKey(label))
        {
            GameObject objectToSpawn;
            bool succesfulDequeue = poolDictionary[label].TryDequeue(out objectToSpawn);
            
            if (!succesfulDequeue)
            {
                // Empty queue, make more
                foreach (var pool in pools)
                {
                    if (pool.label.Equals(label))
                    {
                        pool.size += 20;
                        for (int i = 0; i < 20; i++)
                        {
                            GameObject obj = Instantiate(pool.prefab, groupObject.transform, true);
                            obj.SetActive(false);
                            poolDictionary[label].Enqueue(obj);
                        }

                        poolDictionary[label].TryDequeue(out objectToSpawn);
                        objectToSpawn.SetActive(true);
                    }
                }
            }

            if (objectToSpawn != null)
            {
                if (objectToSpawn.TryGetComponent(out NavMeshAgent navMeshAgent))
                {
                    navMeshAgent.Warp(position);    //make sure that navmesh agent is positioned with warp to avoid problems
                }
                else
                {
                    objectToSpawn.transform.position = position;
                }
                
                objectToSpawn.transform.rotation = rotation;
                objectToSpawn.SetActive(true);
                //TODO Maintain list of all components for more performance
                objectToSpawn.GetComponent<IPooledObject>()?.onObjectSpawn();
                
                if(objectToSpawn.TryGetComponent(out AnimalController animalController))
                {
                    animalController.deadState.onDeath += HandleDeadAnimal;
                    animalController.SpawnNew += HandleBirthAnimal;
                }
                else if (objectToSpawn.TryGetComponent(out PlantController plantController))
                {
                    plantController.SpawnNewPlant += HandleGrowPlant;
                    plantController.onDeadPlant += HandleDeadPlant;
                    dh.LogNewPlant();   //must be placed here because grass dont call on neither handlefoodinstantiated nor handlegrowfood
                }
                
                return objectToSpawn;
            }
            else
            {
                Debug.Log("objectToSpawn " + label + " is null");
            }
        }
        else
        {
            Debug.Log("pooldictionary null: " + poolDictionary != null + " pooldictionary contains key " + label + ": " +poolDictionary.ContainsKey(label));
        }

        Debug.Log("Spawned NULL!");
        return null;
    }

    private void HandleDeadPlant(PlantController plantController)
    {
        if (plantController.TryGetComponent(out IPooledObject pooledObject))
        {
            GameObject plantObj;
            (plantObj = plantController.gameObject).SetActive(false);
            poolDictionary[pooledObject.GetObjectLabel()].Enqueue(plantObj);
            dh.LogDeadPlant();
            plantController.SpawnNewPlant -= HandleGrowPlant;
            plantController.onDeadPlant -= HandleDeadPlant;    
        } else Debug.Log("IpooledObject does not exist on: " + plantController.name);
    }

    public void HandleFoodInstantiated(GameObject o, string label)
    {
        if (poolDictionary != null && poolDictionary.ContainsKey(label))
        {
            o.SetActive(true);
            o.GetComponent<IPooledObject>()?.onObjectSpawn();
            dh.LogNewPlant();
            if (o.TryGetComponent(out PlantController plantController))
            {
                plantController.onDeadPlant += HandleDeadPlant;
                plantController.SpawnNewPlant += HandleGrowPlant;
            }
            if (stackDictionary != null && stackDictionary.ContainsKey(label))
            {
                stackDictionary[label].Push(o);
            }
        }
    }
    
    private void HandleGrowPlant(string label, Vector3 pos)
    {
        
        GameObject newPlant = SpawnFromPool(label, pos, Quaternion.identity);
        if (newPlant != null)
        {
            PlantController plantModel = newPlant.GetComponent<PlantController>();
            plantModel.plantModel.isRegrowing = false;
            plantModel.plantModel.plantAge = 0;
            plantModel.plantModel.nutritionValue = 0;
        } else Debug.Log("Failed to spawn");
    }
}