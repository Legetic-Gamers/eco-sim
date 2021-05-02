using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ViewController
{
    public class HideoutController : MonoBehaviour
    {
        [SerializeField] private Text text;
        private Camera camera;
        private Renderer renderer;
        
        [SerializeField] protected int capacity;
        protected int currentHiders;

        private void Awake()
        {
            camera = Camera.main;   
            renderer = GetComponent<Renderer>();
            if (capacity == 0)
            {
                capacity = 2;
            }
        }

        private void Start()
        {
            UpdateText();
        }
        
        public void EnterHideout()
        {
            currentHiders++;
            UpdateText();
        }

        public void ExitHideout()
        {
            currentHiders--;
            UpdateText();
        }
        
        public bool CanHide(AnimalController animalController)
        {
            return currentHiders < capacity && animalController.animalModel is RabbitModel;
        }

        private void UpdateText()
        {
            if (text)
            {
                text.text = "hiders: " + currentHiders + " / " + capacity;
            }
        }

        private void Update()
        {
            if (renderer && renderer.isVisible)
            {
                LookAtCamera();
            }
        }

        private void LookAtCamera()
        {
            if (camera && text && text.transform)
            {
                text.transform.LookAt(text.transform.position + camera.transform.rotation * Vector3.back, camera.transform.rotation * Vector3.up);
            }
        }
    }
}