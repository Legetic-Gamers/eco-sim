/*
 * Author: Johan A.
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DataCollection
{
    // TODO Add standard deviation, variance and quartiles 
    public class Collector
    {
        // Sampled in time domain, number of animals in the scene. 
        public  List<int> totalAnimalsAlive;
        
        // Index is generation
        public List<int> totalAnimalsAlivePerGeneration;
        
        // Each index contains the mean of that generation, starting from 0. 
        public List<List<float>> rabbitStatsPerGen;
        public List<List<float>> wolfStatsPerGen;
        public List<List<float>> deerStatsPerGen;
        public List<List<float>> bearStatsPerGen;
        
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
        
        public List<int> agePerGeneration;
        */
        
        /// <summary>
        /// Constructor for a collector. Mostly initialize lists. TODO Add constant for mean or median calculation. 
        /// </summary>
        public Collector()
        {
            // Initialize allStatsPerGeneration as list of lists, with the first (0 th) generation set to 0 for all traits. 
            rabbitStatsPerGen = new List<List<float>>(13);
            wolfStatsPerGen = new List<List<float>>(13);
            deerStatsPerGen = new List<List<float>>(13);
            bearStatsPerGen = new List<List<float>>(13);
            
            for (int i = 0; i < 13; i++) rabbitStatsPerGen.Add(new List<float>{0});
            for (int i = 0; i < 13; i++) wolfStatsPerGen.Add(new List<float>{0});
            for (int i = 0; i < 13; i++) deerStatsPerGen.Add(new List<float>{0});
            for (int i = 0; i < 13; i++) bearStatsPerGen.Add(new List<float>{0});

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
        public void CollectBirth(AnimalModel am)
        {
            int gen = am.generation;
            
            // Changes the referenced list depending on the species of the animal. 
            List<List<float>> currentList = new List<List<float>>();
            switch (am)
            {
                case RabbitModel _:
                    currentList = rabbitStatsPerGen;
                    break;
                case WolfModel _:
                    currentList = wolfStatsPerGen;
                    break;
                case DeerModel _:
                    currentList = deerStatsPerGen;
                    break;
                case BearModel _:
                    currentList = bearStatsPerGen;
                    break;
            }

            // Convert the traits to a list which we can easily access. 
            List<float> traitsInAnimal = ConvertTraitsToList(am.traits);
            
            // Add the traits of the animal to each global statistics list. (Extent the list if the generation increases)
            int index = 0;
            foreach (List<float> statList in currentList.Take(12))
            {
                if (gen > statList.Capacity) statList.AddRange(Enumerable.Repeat<float>(0, statList.Count - gen));
                statList[gen] = (statList[gen] * totalAnimalsAlivePerGeneration[gen] + traitsInAnimal[index]) / (totalAnimalsAlivePerGeneration[gen] + 1);
                index++;
            }
            
            //TODO Choose between animals alive per time step or generation. 
            totalAnimalsAlivePerGeneration[gen] += 1;
        }

        public void CollectDeath(AnimalModel am)
        {
            int gen = am.generation;
            
            // Changes the referenced list depending on the species of the animal. 
            List<List<float>> currentList = new List<List<float>>();
            switch (am)
            {
                case RabbitModel _:
                    currentList = rabbitStatsPerGen;
                    break;
                case WolfModel _:
                    currentList = wolfStatsPerGen;
                    break;
                case DeerModel _:
                    currentList = deerStatsPerGen;
                    break;
                case BearModel _:
                    currentList = bearStatsPerGen;
                    break;
            }
            currentList[13][gen] = (currentList[13][gen] * totalAnimalsAlivePerGeneration[gen] + am.age)/ (totalAnimalsAlivePerGeneration[gen] + 1);
            totalAnimalsAlivePerGeneration[gen] -= 1;
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