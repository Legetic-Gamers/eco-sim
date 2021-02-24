/*
 * Author: Johan A.
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DataCollection
{
    public class Collector
    {
        public List<int> totalAnimalsAlive;
        public Collector()
        {
            totalAnimalsAlive = new List<int>();
            totalAnimalsAlive.Add(GameObject.FindGameObjectsWithTag("Animal").Length);
        }

        public void Collect()
        {
            AddTotalAnimals();
        }

        private void AddTotalAnimals()
        {
            totalAnimalsAlive.Add(GameObject.FindGameObjectsWithTag("Animal").Length);
        }
    }
}    