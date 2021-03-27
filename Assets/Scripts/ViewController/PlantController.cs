using System;
using DataCollection;
using Model;
using UnityEngine;
using UnityEngine.AI;

namespace ViewController
{
    public class PlantController : MonoBehaviour
    {
        private TickEventPublisher tickEventPublisher;
        
        public PlantModel plantModel;
        
        private DataHandler dh;
        

        
        
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
        
        private void Grow()
        {
            //TODO make not stupid increment value
            plantModel.plantAge += 2;           
            if (plantModel.plantSize > PlantModel.plantMaxsize) plantModel.plantSize = PlantModel.plantMaxsize;
            else plantModel.plantSize += 2;

            
            // plant has regrown after being eaten
            if (plantModel.isEaten && plantModel.plantSize > 15)
            {
                gameObject.SetActive(true);
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