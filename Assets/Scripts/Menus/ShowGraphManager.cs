using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menus
{
    public class ShowGraphManager : MonoBehaviour
    {
        public GameObject graph;
        private bool isActive = false;
        
        public void OnClick()
        {
            isActive = !isActive;
            graph.GetComponent<Canvas>().enabled = isActive;
            Window_Graph.IsGraphOne = isActive;
        }
    }
}

