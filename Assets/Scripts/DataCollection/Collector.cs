/*
 * Author: Johan A.
 */

using System;
using System.Collections.Generic;
using AnimalsV2;
using UnityEngine;

namespace DataCollection
{
    [Serializable]
    public class Collector
    {
        // Each index is generation, value is the number of animals in that generation. 
        public static List<AnimalController> totalAnimalsPerGeneration;
        
        // Not supposed to be AnimalController, "Type of animal"
        public static Dictionary<AnimalController, int> numberOfEntitiesPerSpecies;
        
        private Collector()
        {
            totalAnimalsPerGeneration = new List<AnimalController>();
            numberOfEntitiesPerSpecies = new Dictionary<AnimalController, int>();
            EventSubscribe();
        }
        
        private void EventSubscribe()
        {
            /* 
             * Subscribe on all events
             */
            Debug.Log("Collector has subscribed to data events. ");
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