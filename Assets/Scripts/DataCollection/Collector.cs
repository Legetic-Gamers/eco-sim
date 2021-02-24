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
            //numberOfEntitiesPerSpecies = new Dictionary<AnimalController, int>();
            totalAnimalsAlive = new List<int>();
            tickEventPublisher = FindObjectOfType<global::TickEventPublisher>();
            EventSubscribe();
        }

        private TickEventPublisher tickEventPublisher;
        // Each index is generation, value is the number of animals in that generation. 
        public static List<int> totalAnimalsAlive;

        // Not supposed to be AnimalController, "Type of animal"
        //public static Dictionary<AnimalController, int> numberOfEntitiesPerSpecies;

        private void EventSubscribe()
        {
            tickEventPublisher.onCollectorUpdate += AddToTotal;
            Debug.Log("Collector has subscribed to data events. ");
        }

        private void AddToTotal()
        {
            totalAnimalsAlive.Add(GameObject.FindGameObjectsWithTag("Animal").Length);
            /*Debug.Log(totalAnimalsAlive.ToString());*/
        }

        private void EventUnsubscribe()
        {
            /* 
             Subscribe on all events
            */
            Debug.Log("Collector has unsubscribed to data events. ");
        }
        /*
         * TODO Add event function definitions which are subscribed to.
         * TDO On event write to JSON-file/s. 
         */
    }
}    