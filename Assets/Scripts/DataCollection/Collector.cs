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
        // Index is generation
        public List<int> totalAnimalsAlivePerGeneration;
        
        // Each index contains the mean of that generation, starting from 0. 
        public List<List<float>> rabbitStatsPerGenMean;
        public List<List<float>> wolfStatsPerGenMean;
        public List<List<float>> deerStatsPerGenMean;
        public List<List<float>> bearStatsPerGenMean;
        
        public List<List<float>> rabbitStatsPerGenVar;
        public List<List<float>> wolfStatsPerGenVar;
        public List<List<float>> deerStatsPerGenVar;
        public List<List<float>> bearStatsPerGenVar;

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
        
        private float m, s;
        
        /// <summary>
        /// Constructor for a collector. Mostly initialize lists. TODO Add constant for mean or median calculation. 
        /// </summary>
        public Collector()
        {
            // Initialize allStatsPerGeneration as list of lists, with the first (0 th) generation set to 0 for all traits. 
            rabbitStatsPerGenMean = new List<List<float>>(13);
            wolfStatsPerGenMean = new List<List<float>>(13);
            deerStatsPerGenMean = new List<List<float>>(13);
            bearStatsPerGenMean = new List<List<float>>(13);
            
            rabbitStatsPerGenVar = new List<List<float>>(13);
            wolfStatsPerGenVar = new List<List<float>>(13);
            deerStatsPerGenVar = new List<List<float>>(13);
            bearStatsPerGenVar = new List<List<float>>(13);
            
            for (int i = 0; i < 13; i++) rabbitStatsPerGenMean.Add(new List<float>{0});
            for (int i = 0; i < 13; i++) wolfStatsPerGenMean.Add(new List<float>{0});
            for (int i = 0; i < 13; i++) deerStatsPerGenMean.Add(new List<float>{0});
            for (int i = 0; i < 13; i++) bearStatsPerGenMean.Add(new List<float>{0});
            
            for (int i = 0; i < 13; i++) rabbitStatsPerGenVar.Add(new List<float>{0});
            for (int i = 0; i < 13; i++) wolfStatsPerGenVar.Add(new List<float>{0});
            for (int i = 0; i < 13; i++) deerStatsPerGenVar.Add(new List<float>{0});
            for (int i = 0; i < 13; i++) bearStatsPerGenVar.Add(new List<float>{0});

            totalAnimalsAlivePerGeneration = new List<int> {GameObject.FindGameObjectsWithTag("Animal").Length};

            m = s = 0;
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
            List<List<float>> animalMean = GetList(am, true);
            List<List<float>> animalVar = GetList(am, false);

            // Convert the traits to a list which we can easily access. 
            List<float> traitsInAnimal = ConvertTraitsToList(am.traits);
            
            // Add the traits of the animal to each global statistics list. (Extent the list if the generation increases)
            int indexMean = 0;
            foreach (List<float> statList in animalMean.Take(11))
            {
                if (gen > statList.Capacity) statList.AddRange(Enumerable.Repeat<float>(0, statList.Count - gen));
                statList[gen] = (statList[gen] * totalAnimalsAlivePerGeneration[gen] + traitsInAnimal[indexMean]) / (totalAnimalsAlivePerGeneration[gen] + 1);
                indexMean++;
            }
            // Add the traits of the animal to each global statistics list. (Extent the list if the generation increases)
            int indexVar = 0;
            foreach (List<float> statList in animalVar.Take(11))
            {
                if (gen > statList.Capacity) statList.AddRange(Enumerable.Repeat<float>(0, statList.Count - gen));
                UpdateVariance(statList[gen], traitsInAnimal[indexVar], totalAnimalsAlivePerGeneration[0]);
                indexVar++;
            }
            
            //TODO Choose between animals alive per time step or generation. 
            totalAnimalsAlivePerGeneration[am.generation] += 1;
        }

        public void CollectDeath(AnimalModel am)
        {
            int gen = am.generation;
            
            // Changes the referenced list depending on the species of the animal. 
            List<List<float>> statList = GetList(am, true);
            
            statList[12][gen] = (statList[12][gen] * totalAnimalsAlivePerGeneration[gen] + am.age)/ (totalAnimalsAlivePerGeneration[gen] + 1);
            totalAnimalsAlivePerGeneration[gen] -= 1;
        }

        private List<List<float>> GetList(AnimalModel am, bool getMeanList)
        {
            List <List<float>> currentList = new List<List<float>>();
            if (getMeanList)
            {
                switch (am)
                {
                    case RabbitModel _:
                        currentList = rabbitStatsPerGenMean;
                        break;
                    case WolfModel _:
                        currentList = wolfStatsPerGenMean;
                        break;
                    case DeerModel _:
                        currentList = deerStatsPerGenMean;
                        break;
                    case BearModel _:
                        currentList = bearStatsPerGenMean;
                        break;
                }
            }
            else
            {
                switch (am)
                {
                    case RabbitModel _:
                        currentList = rabbitStatsPerGenVar;
                        break;
                    case WolfModel _:
                        currentList = wolfStatsPerGenVar;
                        break;
                    case DeerModel _:
                        currentList = deerStatsPerGenVar;
                        break;
                    case BearModel _:
                        currentList = bearStatsPerGenVar;
                        break;
                }
            }

            return currentList;
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
            //totalAnimalsAlive.Add(GameObject.FindGameObjectsWithTag("Animal").Length);
        }

        private float UpdateVariance(float currentVar, float value, int populationSize)
        {
            float old_m = m;
            m = m + (value - m) / populationSize;
            s = currentVar + (value - m) * (value - old_m);
            return s / (populationSize - 1);
        }
    }
}    