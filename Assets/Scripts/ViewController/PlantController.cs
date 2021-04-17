using System;
using System.Collections;
using DataCollection;
using DefaultNamespace;
using Model;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ViewController
{
    public class PlantController : MonoBehaviour, IPooledObject
    {
        private TickEventPublisher tickEventPublisher;
        
        public PlantModel plantModel;

        public Transform centerTransform;
        
        public Action<Vector3> SpawnNewPlant;
        public Action<PlantController> onDeadPlant;

        public void Awake()
        {
            if (centerTransform == null)
            {
                Debug.LogWarning("Center not assigned, defaulting to transform");
                centerTransform = transform;
            }
            plantModel = new PlantModel();
        } 

        public void Start()
        {
            //If there is no object pooler present, we need to call onObjectSpawn through start
            if (FindObjectOfType<ObjectPooler>() == null)
            {
                onObjectSpawn();
            }
        }
        

        public void OnDestroy()
        {
            if (tickEventPublisher)
            {
                tickEventPublisher.onParamTickEvent -= HandleDeathStatus;
                tickEventPublisher.onParamTickEvent -= HandleEaten; 
                tickEventPublisher.onParamTickEvent -= Grow;
            }
        }

        private void EventSubscribe()
        {
            if (tickEventPublisher)
            {
                tickEventPublisher.onParamTickEvent += HandleDeathStatus;  
                tickEventPublisher.onParamTickEvent += HandleEaten;  
                tickEventPublisher.onParamTickEvent += Grow;  
            }
        }
        
        private void SetPhenotype()
        {
            float normalizedValue = 1f / PlantModel.plantMaxsize;
            gameObject.transform.localScale = new Vector3(normalizedValue, normalizedValue,normalizedValue) * plantModel.nutritionValue;
        }
        
        private void Grow()
        {
            
            plantModel.plantAge += 2;
            if (plantModel.nutritionValue > PlantModel.plantMaxsize)
            {
                plantModel.nutritionValue = PlantModel.plantMaxsize;
                //SetPhenotype();
            }
            else plantModel.nutritionValue += 2;

            float r = Random.Range(0, 1f);
            float rx = Random.Range(-10f, 10f);
            float rz = Random.Range(-10f, 10f);
            // chance of reproducing every 2 seconds if age and size restrictions are met.
            //if (plantModel.nutritionValue > 15 && !plantModel.isEaten && r > 0.95) 
            if (plantModel.plantAge > 15 && plantModel.nutritionValue > 15 && !plantModel.isEaten && r > 0.95)
            {
                float height = 0;
                bool isHit = false;

                var position = gameObject.transform.position;
                Vector3 newPosition = new Vector3(position.x + rx, position.y + 100, position.z + rz);
                Ray ray = new Ray(newPosition, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, float.MaxValue))
                {
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        height = hit.point.y;
                        isHit = true;
                    }
                }
                if (!isHit) return;
                SpawnNewPlant?.Invoke(new Vector3(position.x + rx, height, position.z + rz));
            }
        }
        
        private void HandleEaten()
        {
            if (!plantModel.isEaten) return;
            if (gameObject.activeSelf)
            {
                StartCoroutine(Regrow());
            }
        }

        private IEnumerator Regrow()
        {
            gameObject.SetActive(false);
            yield return new WaitForSeconds(30f);
            gameObject.SetActive(true);
        }
    

        private void HandleDeathStatus()
        {
            if (plantModel != null && plantModel.plantAge > PlantModel.plantMaxAge)
            {
                if (plantModel != null && gameObject.activeSelf)
                {
                    onDeadPlant?.Invoke(this);
                    tickEventPublisher.onParamTickEvent -= HandleDeathStatus;
                    tickEventPublisher.onParamTickEvent -= HandleEaten; 
                    tickEventPublisher.onParamTickEvent -= Grow;
                }
            }
        }

        public void onObjectSpawn()
        {
            tickEventPublisher = FindObjectOfType<TickEventPublisher>();
            EventSubscribe();
        }
    }
}