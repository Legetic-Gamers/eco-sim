using System;
using System.Collections.Generic;
using UnityEngine;

namespace ViewController
{
    public class HideoutController : MonoBehaviour
    {
        [SerializeField] protected int capacity;
        protected int currentHiders;

        private void Awake()
        {
            if (capacity == 0)
            {
                capacity = 2;
            }
        }

        public void EnterHideout()
        {
            currentHiders++;
        }

        public void ExitHideout()
        {
            currentHiders--;
        }
        

        public bool CanHide(AnimalController animalController)
        {
            return currentHiders < capacity && animalController.animalModel is RabbitModel;
        }
    }
}