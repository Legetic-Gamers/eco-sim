/*
 * Author: Johan A.
 */

using System.Collections.Generic;
using UnityEngine;

namespace DataCollection
{
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
        public List<int> maxHealthPerGeneration;
        public List<int> maxHydrationPerGeneration;
        public List<float> maxSpeedPerGeneration;
        public List<float> endurancePerGeneration;
        public List<int> ageLimitPerGeneration;
        public List<float> temperatureResistPerGeneration;
        public List<float> desirabilityResistPerGeneration;
        public List<float> viewAnglePerGeneration;
        public List<float> viewRadiusPerGeneration;
        public List<float> hearingRadiusPerGeneration;
        
        public List<int> agePerGeneration;
        */

        /// <summary>
        /// Constructor for a collector. Mostly initialize lists. 
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
            
            // Changes the referenced lists depending on the species of the animal. 
            (List<List<float>> animalMean, List<List<float>> animalVar, List<float> animalTotal) = GetAnimalList(am);

            // Convert the traits to a list which we can easily access. 
            var traitsInAnimal = ConvertTraitsToList(am.traits);

            // Mean and variance running update
            int indexTrait = 0;
            animalTotal[gen] += 1;
            for (int trait = 0; trait < 12; trait++)
            {
                (float mean, float var) =
                    GetNewMeanVariance(animalMean[trait][gen],animalVar[trait][gen], traitsInAnimal[indexTrait], animalTotal[gen]);
                animalMean[trait][gen] = mean;
                animalVar[trait][gen] = var;
                //if(trait == 0 && gen == 0) Debug.Log(mean);
                indexTrait++;
            }
            totalAnimalsAlivePerGeneration[gen] += 1;
            
            //Debug.Log(rabbitStatsPerGenMean[0][0]);
            
            
        }

        /// <summary>
        /// Update the age statistics when animals die. 
        /// </summary>
        /// <param name="am"> Animal Model of killed animal. </param>
        public void CollectDeath(AnimalModel am)
        {
            int gen = am.generation;
            
            // Changes the referenced lists depending on the species of the animal. 
            (List<List<float>> animalMean, List<List<float>> animalVar, List<float> animalTotal) = GetAnimalList(am);

            (float mean, float var) =
                GetNewMeanVariance(animalMean[12][gen], animalVar[12][gen], am.age, animalTotal[gen]);
                
            animalMean[12][gen] = mean;
            animalVar[12][gen] = var;
        }

        /// <summary>
        /// Gets the corresponding statistical lists to each animal model type.
        /// </summary>
        /// <param name="am"> Animal Model to get lists for </param>
        /// <returns> Tuple with mean, variance and total animals alive for that animal type.</returns>
        private (List<List<float>> m, List<List<float>> v, List<float> t) GetAnimalList(AnimalModel am)
        {
            List<List<float>> meanList = new List<List<float>>();
            List<List<float>> varList = new List<List<float>>();
            List<float> totalList = new List<float>();
            switch (am)
                {
                    case RabbitModel _:
                        meanList = rabbitStatsPerGenMean;
                        varList = rabbitStatsPerGenVar;
                        totalList = rabbitTotalAlivePerGen;
                        break;
                    case WolfModel _:
                        meanList = wolfStatsPerGenMean;
                        varList = wolfStatsPerGenVar;
                        totalList = wolfTotalAlivePerGen;
                        break;
                    case DeerModel _:
                        meanList = deerStatsPerGenMean;
                        varList = deerStatsPerGenVar;
                        totalList = deerTotalAlivePerGen;
                        break;
                    case BearModel _:
                        meanList = bearStatsPerGenMean;
                        varList = bearStatsPerGenVar;
                        totalList = bearTotalAlivePerGen;
                        break;
                }

            return (meanList, varList, totalList);
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
        
        /// <summary>
        /// Calculates the running mean and variance of a given data set.
        /// </summary>
        /// <param name="m"> Current mean in the set. </param>
        /// <param name="s"> Current variance in the set. </param>
        /// <param name="valueToAdd"> Value to be added to the set. </param>
        /// <param name="populationSize"> Number of samples in set. </param>
        /// <returns> Tuple with the updated mean and variance. </returns>
        // source: https://www.johndcook.com/blog/standard_deviation/
        private (float m, float v) GetNewMeanVariance(float m, float s,float valueToAdd, float populationSize)
        {
            var oldM = m;
            if(populationSize > 1) s = s * (populationSize - 2);
            m += (valueToAdd - m) / populationSize;
            s += (valueToAdd - m) * (valueToAdd - oldM);
            return (populationSize > 1) ? (m, s / (populationSize - 1f)) : (m, 0);
        }
    }
}    