/*
 * Author: Johan A.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DataCollection
{
    public class Collector
    {
        public List<int> totalAnimalsAlive;
        
        //Index is generation
        public List<int> totalAnimalsAlivePerGeneration;
        public List<List<float>> allStatsPerGeneration;
        
        /*
        public List<float> sizePerGeneration;
        public List<float> maxEnergyPerGeneration;
        public List<float> maxHelathPerGeneration;
        public List<float> maxHydrationPerGeneration;
        public List<float> maxSpeedPerGeneration;
        public List<float> edurancePerGeneration;
        public List<float> ageLimitPerGeneration;
        public List<float> temperatureResistPerGeneration;
        public List<float> desirabilityResistPerGeneration;
        public List<float> viewAnglePerGeneration;
        public List<float> viewRadiusPerGeneration;
        public List<float> hearingRadiusPerGeneration;
        public List<float> behaviorTypePerGeneration;
        public List<float> speciesPerGeneration;
        */
        /*
        float size,
        int maxEnergy, 
        int maxHealth,
        int maxHydration,
        float maxSpeed,
        float endurance, 
        int ageLimit, 
        float temperatureResist, 
        float desirability, 
        float viewAngle, 
        float viewRadius, 
        float hearingRadius,
        BehaviorType behaviorType,
        Species species
        */
        
        /// <summary>
        /// Constructor for a collector. Mostly initialize lists. 
        /// </summary>
        public Collector()
        {
            allStatsPerGeneration = new List<List<float>>(14);
            totalAnimalsAlive = new List<int> {GameObject.FindGameObjectsWithTag("Animal").Length};
            totalAnimalsAlivePerGeneration = new List<int> {GameObject.FindGameObjectsWithTag("Animal").Length};
        }
        
        /// <summary>
        /// Overloaded Collect methods
        /// </summary>
        public void Collect()
        {
            GameObject[] allAnimalsAlive = GameObject.FindGameObjectsWithTag("Animal");
            //AddTotalAnimals();
            AddToTraits(allAnimalsAlive);
        }

        public void AddToTraits(GameObject[] allAnimals)
        {
            List<float> tratisInAnimal = ConvertTraitsToList(am.traits);
            totalAnimalsAlivePerGeneration[am.generation] += 1;
            int index = 0;
            foreach (var list in allStatsPerGeneration)
            {
                list[am.generation] = list[am.generation] + tratisInAnimal[index];
                index += 1;
            }
        }

        private List<float> ConvertTraitsToList(Traits classTraits)
        {
            List<float> traits = new List<float>();
            List<object> traitObjects = traits.GetType()
                .GetFields()
                .Select(field => field.GetValue(traits))
                .ToList();
            foreach (var t in traitObjects)
            {
                if (t.GetType().IsPrimitive) traits[0] = (float) t;
            }

            return traits;
        }

        private void AddTotalAnimals()
        {
            totalAnimalsAlive.Add(GameObject.FindGameObjectsWithTag("Animal").Length);
        }

    }
}    