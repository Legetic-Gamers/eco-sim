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
        private Collector c;
        public Action<List<int>> Display;
        private void Awake()
        {
            c = new Collector();
            tickEventPublisher = FindObjectOfType<global::TickEventPublisher>();
            tickEventPublisher.onCollectorUpdate += UpdateDataAndGraph;
        }
        
        private void UpdateDataAndGraph()
        {
            c.Collect();
            if(c.totalAnimalsAlive != null) Display(c.totalAnimalsAlive);
        }
    }
}