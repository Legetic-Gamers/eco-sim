using System;
using DataCollection;
using Model;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace ViewController
{
    public class PlantController : MonoBehaviour
    {
        private TickEventPublisher tickEventPublisher;
        
        public PlantModel plantModel;
        
        private DataHandler dh;
        
<<<<<<< HEAD
        public Transform centerTransform;

        public void Awake()
        {
            if (centerTransform == null)
            {
                Debug.LogWarning("Center not assigned, defaulting to transform");
                centerTransform = transform;
            }
        }

=======
        
        
>>>>>>> dynamic-food
        public void Start()
        {
            tickEventPublisher = FindObjectOfType<global::TickEventPublisher>();
            plantModel = new PlantModel();
            EventSubscribe();

            dh = FindObjectOfType<DataHandler>();
            dh.LogNewPlant(plantModel);
        }

        public void OnDestroy()
        {
            EventUnSubscribe();
        }

        private void EventSubscribe()
        {
            if (tickEventPublisher)
            {
                tickEventPublisher.onParamTickEvent += HandleDeathStatus;
                tickEventPublisher.onParamTickEvent += HandleEaten; //TODO HandleEaten should be called somewhere else.
                tickEventPublisher.onParamTickEvent += Grow;
            }
        }

        private void EventUnSubscribe()
        {
            if (tickEventPublisher)
            {
                tickEventPublisher.onParamTickEvent -= HandleDeathStatus;
                tickEventPublisher.onParamTickEvent -= HandleEaten; //TODO this does not belong here.
                tickEventPublisher.onParamTickEvent -= Grow;
            }
        }
        
        private void SetPhenotype()
        {
            float normalizedValue = 1f / PlantModel.plantMaxsize;
            gameObject.transform.localScale = new Vector3(normalizedValue, normalizedValue,normalizedValue) * plantModel.plantSize;
        }
        
        private void Grow()
        {
            //TODO make not stupid increment value
            plantModel.plantAge += 2;
            if (plantModel.plantSize > PlantModel.plantMaxsize)
            {
                plantModel.plantSize = PlantModel.plantMaxsize;
                SetPhenotype();
            }
            else plantModel.plantSize += 2;
            

            
            // plant has regrown after being eaten
            if (plantModel.isEaten && plantModel.plantSize > 15)
            {
                gameObject.SetActive(true);
                SetPhenotype();
            }

            float r = Random.Range(0, 1f);
            float rx = Random.Range(-5f, 5f);
            float rz = Random.Range(-5f, 5f);
            // 5% chance of reproducing every 2 seconds if age and size restrictions are met.
            if (plantModel.plantAge > 30 && plantModel.plantSize > 15 && !plantModel.isEaten && r > 0.95) 
            {
                float height = 0;
                bool isHit = false;
                
                var position = gameObject.transform.position;
                Vector3 newPosition = new Vector3(position.x + rx, position.y + 100, position.z + rz);
                Ray ray = new Ray(newPosition, gameObject.transform.TransformDirection(Vector3.down));
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
                //GameObject offspring = Instantiate(gameObject, gameObject.transform, true);
                GameObject offspring = Instantiate(gameObject);
                offspring.transform.position = new Vector3(position.x + rx, height, position.z + rz);
                PlantModel offspringModel = new PlantModel();
                offspring.GetComponent<PlantController>().plantModel = offspringModel;
            }


        }



        private void HandleEaten()
        {
            if (!plantModel.isEaten) return;
            gameObject.SetActive(false);
            
        }

        private void HandleDeathStatus()
        {
            if (plantModel != null && plantModel.plantAge > PlantModel.plantMaxAge)
            {
                dh.LogDeadPlant(plantModel);
                Destroy(gameObject);
            }
        }
    }
}