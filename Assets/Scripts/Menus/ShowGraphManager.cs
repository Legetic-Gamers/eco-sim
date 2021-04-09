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


        // Toggle if graph visibility
        public void OnClick()
        {
            isActive = !isActive;
            canvas.GetComponent<GraphicRaycaster>().enabled = isActive;
            graph.GetComponent<Canvas>().enabled = isActive;
            Window_Graph.IsGraphOne = isActive;
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

