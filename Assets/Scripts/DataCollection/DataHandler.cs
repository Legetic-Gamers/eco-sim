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

        public void LogNewAnimal(AnimalModel animalModel)
        {
            c.AddNewAnimal(animalModel);
        }

        private void UpdateDataAndGraph()
        {
            c.Collect();
            // Temp casting
            List<int> sizePerGeneration = new List<int>();
            foreach (float f in c.allStatsPerGeneration[0])
            {
                sizePerGeneration.Add((int) f);
            }
            Display(sizePerGeneration);
            WriteToFile(c.totalAnimalsAlive);
        }
    }
}