/*
 * Author: Johan A.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
        
        public List<float> rabbitTotalAlivePerGen;
        public List<float> wolfTotalAlivePerGen;
        public List<float> deerTotalAlivePerGen;
        public List<float> bearTotalAlivePerGen;

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
            
            rabbitTotalAlivePerGen = new List<float>();
            wolfTotalAlivePerGen = new List<float>();
            deerTotalAlivePerGen = new List<float>();
            bearTotalAlivePerGen = new List<float>();
            
            rabbitTotalAlivePerGen.Add(0);
            wolfTotalAlivePerGen.Add(0);
            deerTotalAlivePerGen.Add(0);
            bearTotalAlivePerGen.Add(0);
            
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
            var animalMean = GetMeanList(am);
            var animalVar = GetVarList(am);
            var animalTotal = GetTotalList(am);

            // Convert the traits to a list which we can easily access. 
            var traitsInAnimal = ConvertTraitsToList(am.traits);

            // Mean
            int indexTrait = 0;
            for (int trait = 0; trait < 12; trait++)
            {
                animalMean[trait][gen] = (animalMean[trait][gen] * animalTotal[gen] + traitsInAnimal[indexTrait]) / (animalTotal[gen] + 1.0f);
                indexTrait++;
            }

            // Variance
            indexTrait = 0;
            for (int trait = 0; trait < 12; trait++)
            {
                animalVar[trait][gen] = GetNewVariance(animalVar[trait][gen], traitsInAnimal[indexTrait], animalTotal[gen]);
                indexTrait++;
            }
            
            animalTotal[gen] += 1;
            totalAnimalsAlivePerGeneration[gen] += 1;
        }

        public void CollectDeath(AnimalModel am)
        {
            int gen = am.generation;
            
            // Changes the referenced list depending on the species of the animal. 
            List<List<float>> statList = GetMeanList(am);
            
            // Mean age
            statList[12][gen] = (statList[12][gen] * totalAnimalsAlivePerGeneration[gen] + am.age)/ (totalAnimalsAlivePerGeneration[gen] + 1);
        }

        private List<List<float>> GetMeanList(AnimalModel am)
        {
            switch (am)
                {
                    case RabbitModel _:
                        return(rabbitStatsPerGenMean);
                    case WolfModel _:
                        return(wolfStatsPerGenMean);
                    case DeerModel _:
                        return(deerStatsPerGenMean);
                    case BearModel _:
                        return(bearStatsPerGenMean);
                }

            return null;
        }
        
        private List<List<float>> GetVarList(AnimalModel am)
        {
            switch (am)
            {
                case RabbitModel _:
                    return(rabbitStatsPerGenVar);
                case WolfModel _:
                    return(wolfStatsPerGenVar);
                case DeerModel _:
                    return(deerStatsPerGenVar);
                case BearModel _:
                    return(bearStatsPerGenVar);
            }

            return null;
        }
        
        private List<float> GetTotalList(AnimalModel am)
        {
            switch (am)
            {
                case RabbitModel _:
                    return(rabbitTotalAlivePerGen);
                case WolfModel _:
                    return(wolfTotalAlivePerGen);
                case DeerModel _:
                    return(deerTotalAlivePerGen);
                case BearModel _:
                    return(bearTotalAlivePerGen);
            }

            return null;
        }
        
        /// <summary>
        /// Converts the class traits to a regular list of floats for easy access. 
        /// </summary>
        /// <param name="classTraits"> Type Traits; Traits of an animal model</param>
        /// <returns> List containing the float value of each trait, see top of class for order of traits. </returns>
        private List<float> ConvertTraitsToList(Traits classTraits)
        {
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

        private float GetNewVariance(float currentVariance, float valueToAdd, float populationSize)
        {
            float oldM = m;
            m = m + (valueToAdd - m) / populationSize;
            s = currentVariance + (valueToAdd - m) * (valueToAdd - oldM);
            return s / (populationSize - 1);
        }
    }
}    