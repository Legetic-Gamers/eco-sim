using System;
using DataCollection;
using Model;
using UnityEngine;

namespace ViewController
{
    public class PlantController : MonoBehaviour
    {
        private TickEventPublisher tickEventPublisher;
        
        public PlantModel plantModel;

        private DataHandler dh;
        
        public void Start()
        {
            plantModel = new PlantModel();
            if (tickEventPublisher)
            {
                tickEventPublisher = FindObjectOfType<TickEventPublisher>();
                tickEventPublisher.onParamTickEvent += HandleDeathStatus;    
            }

            dh = FindObjectOfType<DataHandler>();
            dh.LogNewPlant(plantModel);
        }

        public void OnDestroy()
        {
            if (tickEventPublisher)
            {
                tickEventPublisher.onParamTickEvent -= HandleDeathStatus;
            }
        }

        private void HandleDeathStatus()
        {
            if (plantModel != null && plantModel.isEaten)
            {
                dh.LogDeadPlant(plantModel);
                Destroy(gameObject);
            }
        }
    }
}