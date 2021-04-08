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
            Debug.Log("Clicked");
            graph.SetActive(!isActive);
            isActive = !isActive;
        }
    }
}

