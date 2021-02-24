/*
 * Author: Johan A.
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DataCollection
{
    public class Collector : MonoBehaviour
    {
        private void Awake()
        {
            totalAnimalsAlive = new List<int>();
            totalAnimalsAlive.Add(GameObject.FindGameObjectsWithTag("Animal").Length);
            tickEventPublisher = FindObjectOfType<global::TickEventPublisher>();
            EventSubscribe();
        }

        private TickEventPublisher tickEventPublisher;
        
        // Each index is generation, value is the number of animals in that generation. 
        public List<int> totalAnimalsAlive;
        

        private void EventSubscribe()
        {
            tickEventPublisher.onCollectorUpdate += AddToTotal;
        }

        private void AddToTotal()
        {
            totalAnimalsAlive.Add(GameObject.FindGameObjectsWithTag("Animal").Length);
        }

        private void EventUnsubscribe()
        {
            
        }
    }
}    