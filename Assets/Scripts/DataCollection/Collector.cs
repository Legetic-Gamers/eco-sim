/*
 * Author: Johan A.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

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
        public List<int> maxEnergyPerGeneration;
        public List<int> maxHelathPerGeneration;
        public List<int> maxHydrationPerGeneration;
        public List<float> maxSpeedPerGeneration;
        public List<float> edurancePerGeneration;
        public List<int> ageLimitPerGeneration;
        public List<float> temperatureResistPerGeneration;
        public List<float> desirabilityResistPerGeneration;
        public List<float> viewAnglePerGeneration;
        public List<float> viewRadiusPerGeneration;
        public List<float> hearingRadiusPerGeneration;
        public List<BehaviorType> behaviorTypePerGeneration;
        public List<Species> speciesPerGeneration;
        BehaviorType behaviorType,
        Species species
        */
        
        /// <summary>
        /// Constructor for a collector. Mostly initialize lists. 
        /// </summary>
        public Collector()
        {
            allStatsPerGeneration = new List<List<float>>(12);
            for (int i = 0; i < 12; i++)
            {
                allStatsPerGeneration.Add(new List<float>(1));
            }
            //totalAnimalsAlive = new List<int> {GameObject.FindGameObjectsWithTag("Animal").Length};
            totalAnimalsAlivePerGeneration = new List<int> {GameObject.FindGameObjectsWithTag("Animal").Length};
        }
        
        /// <summary>
        /// Overloaded Collect methods
        /// </summary>
        public void Collect()
        {
            //GameObject[] allAnimalsAlive = GameObject.FindGameObjectsWithTag("Animal");
            //AddTotalAnimals();
        }

        public void AddNewAnimal(AnimalModel am)
        {
            int gen = am.generation;
            totalAnimalsAlivePerGeneration[gen] += 1;
            List<float> traitsInAnimal = ConvertTraitsToList(am.traits);
            
            int index = 0;
            foreach (List<float> statList in allStatsPerGeneration)
            {
                if (gen > statList.Count) statList.AddRange(Enumerable.Repeat<float>(0,statList.Count-gen));
                else statList[gen] = statList[gen]+ traitsInAnimal[index];
                index += 1;
            }
        }

        private List<float> ConvertTraitsToList(Traits classTraits)
        {
            // Yikes, did not find another working way
            List<float> traits = new List<float>
            {
                classTraits.size,
                classTraits.maxEnergy,
                classTraits.maxHealth,
                classTraits.maxHydration,
                classTraits.maxSpeed,
                classTraits.endurance,
                classTraits.ageLimit,
                classTraits.temperatureResist,
                classTraits.desirability,
                classTraits.viewAngle,
                classTraits.viewRadius,
                classTraits.hearingRadius,
            };
            return traits;
        }

        private void AddTotalAnimals()
        {
            totalAnimalsAlive.Add(GameObject.FindGameObjectsWithTag("Animal").Length);
        }

    }
}    