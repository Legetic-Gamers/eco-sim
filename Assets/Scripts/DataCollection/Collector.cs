/*
 * Author: Johan A.
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DataCollection
{
    public class Collector
    {
        // Sampled in time domain, number of animals in the scene. 
        public  List<int> totalAnimalsAlive;
        
        //Index is generation
        public List<int> totalAnimalsAlivePerGeneration;
        public List<List<float>> allStatsPerGeneration;
        
        /*
        These lists are contained in allStatsPerGeneration in order:
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
            // Initialize allStatsPerGeneration as list of lists, with the first (0 th) generation set to 0 for all traits. 
            allStatsPerGeneration = new List<List<float>>(12);
            for (int i = 0; i < 12; i++)
            {
                allStatsPerGeneration.Add(new List<float>{0});
            }
            totalAnimalsAlive = new List<int> {GameObject.FindGameObjectsWithTag("Animal").Length};
            totalAnimalsAlivePerGeneration = new List<int> {GameObject.FindGameObjectsWithTag("Animal").Length};
        }
        
        /// <summary>
        /// Collect is called at even time intervals by the TickEventPublisher
        /// </summary>
        public void Collect()
        {
            AddTotalAnimals();
        }
        
        /// <summary>
        /// Collect each animal model containing its traits. 
        /// </summary>
        /// <param name="am"> Animal Model containing traits.</param>
        public void Collect(AnimalModel am)
        {
            int gen = am.generation;
            
            //TODO Choose between animals alive per time step or generation. 
            totalAnimalsAlivePerGeneration[gen] += 1;
            
            // Convert the traits to a list which we can easily access. 
            List<float> traitsInAnimal = ConvertTraitsToList(am.traits);
            
            // Add the traits of the animal to each global statistics list. (Extent the list if the generation increases)
            int index = 0;
            foreach (List<float> statList in allStatsPerGeneration)
            {
                if (gen > statList.Capacity) statList.AddRange(Enumerable.Repeat<float>(0, statList.Count - gen));
                statList[gen] += traitsInAnimal[index];
                index++;
            }
        }
        
        /// <summary>
        /// Converts the class traits to a regular list of floats for easy access. 
        /// </summary>
        /// <param name="classTraits"> Type Traits; Traits of an animal model</param>
        /// <returns> List containing the float value of each trait, see top of class for order of traits. </returns>
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
        
        /// <summary>
        /// Check the entire scene for all objects with the "Animal" tag and add at the end of the time series list. 
        /// </summary>
        private void AddTotalAnimals()
        {
            totalAnimalsAlive.Add(GameObject.FindGameObjectsWithTag("Animal").Length);
        }

    }
}    