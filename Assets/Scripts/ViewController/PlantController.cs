using System;
using Model;
using UnityEngine;

namespace ViewController
{
    public class PlantController : MonoBehaviour
    {
        private TickEventPublisher tickEventPublisher;
        
        public PlantModel plantModel;
        public void Start()
        {
            plantModel = new PlantModel();
            tickEventPublisher = FindObjectOfType<TickEventPublisher>();
            tickEventPublisher.onParamTickEvent += HandleDeathStatus;
        }

        public void OnDestroy()
        {
            tickEventPublisher.onParamTickEvent -= HandleDeathStatus;
        }

        private void HandleDeathStatus()
        {
            if (plantModel != null && plantModel.isEaten)
            {
                Destroy(gameObject);
            }
        }
    }
}