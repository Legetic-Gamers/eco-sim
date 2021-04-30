using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using ViewController;
using Random = UnityEngine.Random;

public class GrassProducer : MonoBehaviour
{
    private TickEventPublisher tickEventPublisher;

    [SerializeField]
    private GameObject grassPrefab;
    
    private PlantController grassController;

    private ObjectPooler objectPooler;

    private int grassCount;
    
    private const int maxGrassCount = 1;
    private void Start()
    {
        tickEventPublisher = FindObjectOfType<TickEventPublisher>();
        tickEventPublisher.onParamTickEvent += GrowGrassNearby;
        objectPooler = FindObjectOfType<ObjectPooler>();
        
        if (grassPrefab.TryGetComponent(out PlantController plantController))
        {
            grassController = plantController;
        } else Debug.Log("NO PLANTCONTROLLER ATTACHED");

        grassCount = 0;
    }

    private void GrowGrassNearby()
    {
        if (grassCount < maxGrassCount)
        {
            float chance = Random.Range(0, 1f);
            if (chance > 0.995)
            {
                if (grassPrefab)
                {
                
                    Vector3 position = transform.position;
                    float rx = Random.Range(-10f, 10f);
                    float rz = Random.Range(-10f, 10f);
                    Vector3 newPosition = new Vector3(position.x + rx, position.y + 100, position.z + rz);
                
                
                    Ray ray = new Ray(newPosition, Vector3.down);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, float.MaxValue))
                    {
                        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
                        {
                            newPosition = hit.point;
                        
                            if (objectPooler)
                            {
                                GameObject producedGrass = objectPooler.SpawnFromPool(grassController.GetObjectLabel(), newPosition, Quaternion.identity);
                                grassCount++;
                                //Debug.Log("spawning grass");

                                if (producedGrass.TryGetComponent(out PlantController plantController))
                                {
                                    plantController.onDeadPlant += decrementGrassCountAndUnsubscribe;
                                    StartCoroutine(ExpirePlant(plantController));
                                }
                            }
                        
                        }
                    }
                } else Debug.Log("NO GRASS PREFAB ON WATER BLOCK");    
            }    
        }
    }

    IEnumerator ExpirePlant(PlantController plantController)
    {
        yield return new WaitForSeconds(60f);
        plantController.GetEaten(); //just remove the food
    }
    
    

    private void decrementGrassCountAndUnsubscribe(PlantController plantController)
    {
        grassCount--;
        //unsubscribe this action
        plantController.onDeadPlant -= decrementGrassCountAndUnsubscribe;
    }

    private void OnDestroy()
    {
        if(tickEventPublisher) tickEventPublisher.onParamTickEvent -= GrowGrassNearby;
    }
}
