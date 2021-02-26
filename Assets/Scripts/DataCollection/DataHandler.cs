/*
 * Author: Johan A. 
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using static DataCollection.Formatter;

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
            // Temp casting
            List<int> statsInInteger = new List<int>();
            foreach (var f in c.allStatsPerGeneration[0])
            {
                statsInInteger.Add((int) f);   
            }
            
            Display(statsInInteger);
            WriteToFile(c.totalAnimalsAlive);
        }
    }
}