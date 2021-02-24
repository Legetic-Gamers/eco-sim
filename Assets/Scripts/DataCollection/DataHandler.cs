using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataCollection.Collector;

namespace DataCollection
{
    public class DataHandler : MonoBehaviour
    {
        private TickEventPublisher tickEventPublisher;
        public Collector c;
        public Action<List<int>> Display;
        private void Awake()
        {
            tickEventPublisher = FindObjectOfType<global::TickEventPublisher>();
            tickEventPublisher.onCollectorUpdate += UpdateGraph;
        }
        
        private void UpdateGraph()
        {
            if(c.totalAnimalsAlive != null) Display(c.totalAnimalsAlive);
            
        }
    }
}