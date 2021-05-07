using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menus
{
    public class ShowGraphManager : MonoBehaviour
    {
        public GameObject canvas;
        public GameObject graph;
        private bool isActive = false;

        private bool isRabbits = false;
        private bool isWolves = false;
        private bool isDeer = false;
        private bool isBears = false;

        public Action SetDropDownValues;
        


        // Toggle if graph visibility
        public void OnClick()
        {
            isActive = !isActive;
            canvas.GetComponent<GraphicRaycaster>().enabled = isActive;
            graph.GetComponent<Canvas>().enabled = isActive;
            Window_Graph.IsGraphOne = isActive;
            

            if (isActive)
            {
                if (FindObjectOfType<RabbitController>()) isRabbits = true;
                if (FindObjectOfType<WolfController>()) isWolves = true;
                if (FindObjectOfType<DeerController>()) isDeer = true;
                if (FindObjectOfType<BearController>()) isBears = true;
            }
            SetDropDownValues();
        }

        // Use from GameMenuManager to hide graph
        public void HideGraph()
        {
            isActive = false;
            canvas.GetComponent<GraphicRaycaster>().enabled = false;
            graph.GetComponent<Canvas>().enabled = false;
            Window_Graph.IsGraphOne = false;
        }


    }
}

