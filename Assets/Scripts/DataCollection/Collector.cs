/*
 * Author: Johan A.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Model;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DataCollection
{
    public class Collector
    {
        private int cap = 13;
        public Action onAllExtinct;

        public int totalAnimalsAlive;
        
        // Index is generation
        public readonly List<int> totalAnimalsAlivePerGeneration;
        
        public readonly List<List<float>> birthRatePerMinute;
        public readonly List<float> newBirths;
        
        private int timeIndexBirth;
        private int timeIndexFood;

        // Each index contains the mean of that generation, starting from 0. 
        public readonly List<List<float>> rabbitStatsPerGenMean;
        public readonly List<List<float>> wolfStatsPerGenMean;
        public readonly List<List<float>> deerStatsPerGenMean;
        public readonly List<List<float>> bearStatsPerGenMean;
        
        public readonly List<List<float>> rabbitStatsPerGenVar;
        public readonly List<List<float>> wolfStatsPerGenVar;
        public readonly List<List<float>> deerStatsPerGenVar;
        public readonly List<List<float>> bearStatsPerGenVar;
        
        public readonly List<float> rabbitTotalAlivePerGen;
        public readonly List<float> wolfTotalAlivePerGen;
        public readonly List<float> deerTotalAlivePerGen;
        public readonly List<float> bearTotalAlivePerGen;

        public readonly List<int> currentAnimalsTotalAlivePerSpecies;

        public readonly List<float> totalDeadAnimals; 
        
        public readonly List<int> foodActivePerMinute;
        
        //Special, index is the cause and the content is the total dead of that cause
        public Dictionary<AnimalModel.CauseOfDeath, int> causeOfDeath;

        /*
        These lists are contained in allStatsPerGeneration in order:
        public List<float> sizePerGeneration;
        public List<int> maxEnergyPerGeneration;
        public List<int> maxHealthPerGeneration;
        public List<int> maxHydrationPerGeneration;
        public List<float> maxSpeedPerGeneration;
        public List<float> endurancePerGeneration;
        public List<int> ageLimitPerGeneration;
        public List<float> desirabilityResistPerGeneration;
        public List<float> viewAnglePerGeneration;
        public List<float> viewRadiusPerGeneration;
        public List<float> hearingRadiusPerGeneration;
        public List<float> movementFromStartToEndPerGeneration;
        Death:
        public List<int> agePerGeneration;
        */

        /// <summary>
        /// Constructor for a collector. Mostly initialize lists. 
        /// </summary>
        public Collector()
        {
            
            // Initialize allStatsPerGeneration as list of lists, with the first (0 th) generation set to 0 for all traits. 
            rabbitStatsPerGenMean = new List<List<float>>();
            wolfStatsPerGenMean = new List<List<float>>();
            deerStatsPerGenMean = new List<List<float>>();
            bearStatsPerGenMean = new List<List<float>>();
            
            rabbitStatsPerGenVar = new List<List<float>>();
            wolfStatsPerGenVar = new List<List<float>>();
            deerStatsPerGenVar = new List<List<float>>();
            bearStatsPerGenVar = new List<List<float>>();
            
            for (int i = 0; i < cap; i++) rabbitStatsPerGenMean.Add(new List<float>{0, 0, 0, 0});
            for (int i = 0; i < cap; i++) wolfStatsPerGenMean.Add(new List<float>{0, 0, 0, 0});
            for (int i = 0; i < cap; i++) deerStatsPerGenMean.Add(new List<float>{0, 0, 0, 0});
            for (int i = 0; i < cap; i++) bearStatsPerGenMean.Add(new List<float>{0, 0, 0, 0});
            
            for (int i = 0; i < cap; i++) rabbitStatsPerGenVar.Add(new List<float>{0, 0, 0, 0});
            for (int i = 0; i < cap; i++) wolfStatsPerGenVar.Add(new List<float>{0, 0, 0, 0});
            for (int i = 0; i < cap; i++) deerStatsPerGenVar.Add(new List<float>{0, 0, 0, 0});
            for (int i = 0; i < cap; i++) bearStatsPerGenVar.Add(new List<float>{0, 0, 0, 0});

            totalAnimalsAlivePerGeneration = new List<int> {0};

            birthRatePerMinute = new List<List<float>>(4);
            for (int i = 0; i < 4; i++) birthRatePerMinute.Add(new List<float>{0});
            
            rabbitTotalAlivePerGen = new List<float>();
            wolfTotalAlivePerGen = new List<float>();
            deerTotalAlivePerGen = new List<float>();
            bearTotalAlivePerGen = new List<float>();
            
            rabbitTotalAlivePerGen.Add(0);
            wolfTotalAlivePerGen.Add(0);
            deerTotalAlivePerGen.Add(0);
            bearTotalAlivePerGen.Add(0);

            currentAnimalsTotalAlivePerSpecies = new List<int>{0,0,0,0};

            foodActivePerMinute = new List<int>{0};

            newBirths = new List<float>{0,0,0,0};
            timeIndexBirth = 0;
            timeIndexFood = 0;

            causeOfDeath = new Dictionary<AnimalModel.CauseOfDeath, int>();
            causeOfDeath.Add(AnimalModel.CauseOfDeath.Eaten, 0);
            causeOfDeath.Add(AnimalModel.CauseOfDeath.Hydration, 0);
            causeOfDeath.Add(AnimalModel.CauseOfDeath.Age, 0);
            causeOfDeath.Add(AnimalModel.CauseOfDeath.Energy, 0);
            causeOfDeath.Add(AnimalModel.CauseOfDeath.Hunger, 0);
            causeOfDeath.Add(AnimalModel.CauseOfDeath.Health, 0);
            
            totalDeadAnimals = new List<float>();

            totalAnimalsAlive = 0;
        }
        
        /// <summary>
        /// Collect is called at even time intervals by the TickEventPublisher
        /// </summary>
        public void Collect()
        {
            // Calculate birthRate
            // Birth rate := new births during last minute divided by the already existing animals - the new animals 
            for (int i = 0; i < 4; i++)
            {
                float birthRate = newBirths[i] / (currentAnimalsTotalAlivePerSpecies[i] - newBirths[i]);
                if(birthRatePerMinute[i].Count <= timeIndexBirth) birthRatePerMinute[i].Add(birthRate);
                birthRatePerMinute[i][timeIndexBirth] = birthRate;
                newBirths[i] = 0;
            }
            timeIndexBirth++;
            foodActivePerMinute.Add(foodActivePerMinute[timeIndexFood]);
            timeIndexFood++;
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
            
            if(animalTotal.Count <= gen) animalTotal.Add(1);
            else animalTotal[gen] += 1;

            for (int trait = 0; trait < 11; trait++)
            {
                if(animalMean[trait].Count <= gen) animalMean[trait].Add(0);
                if(animalVar[trait].Count <= gen) animalVar[trait].Add(0);
                (float mean, float var) =
                        GetNewMeanVariance(animalMean[trait][gen],animalVar[trait][gen], traitsInAnimal[indexTrait], animalTotal[gen]);
                if(animalMean.Count <= gen) animalMean[trait].Add(mean);
                else animalMean[trait][gen] = mean;
                if(animalVar.Count <= gen) animalVar[trait].Add(var);
                else animalVar[trait][gen] = var;
                indexTrait++;
            }
            
            // Update the number of births this minute
            UpdateBirths(am);
            
            // Finally add to the total of animals
            if (totalAnimalsAlivePerGeneration.Count <= gen) totalAnimalsAlivePerGeneration.Add(1);
            else totalAnimalsAlivePerGeneration[gen] += 1;

            totalAnimalsAlive++;
        }

        /// <summary>
        /// Match to the model of the animal and update each corresponding list of population
        /// </summary>
        /// <param name="am"> Animal model to change population statistics of </param>
        private void UpdateBirths(AnimalModel am)
        {
            switch (am)
            {
                case RabbitModel _ :
                    newBirths[0] += 1;
                    currentAnimalsTotalAlivePerSpecies[0] += 1;
                    break;
                case WolfModel _ :
                    newBirths[1] += 1;
                    currentAnimalsTotalAlivePerSpecies[1] += 1;
                    break;
                case DeerModel _ :
                    newBirths[2] += 1;
                    currentAnimalsTotalAlivePerSpecies[2] += 1;
                    break;
                case BearModel _ :
                    newBirths[3] += 1;
                    currentAnimalsTotalAlivePerSpecies[3] += 1;
                    break;
            }
        }

        /// <summary>
        /// Update the age statistics when animals die. 
        /// </summary>
        /// <param name="am"> Animal Model of killed animal. </param>
        /// <param name="cause"> Type of death </param>
        /// <param name="distanceTravelled"> Distance travelled to log </param>
        public void CollectDeath(AnimalModel am, AnimalModel.CauseOfDeath cause, float distanceTravelled)
        {
            int gen = am.generation;

            for (int i = totalDeadAnimals.Count - 1; i <= gen; i++) totalDeadAnimals.Add(0);

            totalDeadAnimals[gen] += 1;
            
            // Changes the referenced lists depending on the species of the animal. 
            (List<List<float>> animalMean, List<List<float>> animalVar, _) = GetAnimalList(am);

            for (int i = animalMean[12].Count - 1; i <= gen; i++) animalMean[12].Add(0);
            for (int i = animalMean[12].Count - 1; i <= gen; i++) animalVar[12].Add(0);
                
            for (int i = animalMean[11].Count - 1; i <= gen; i++) animalMean[11].Add(0);
            for (int i = animalMean[11].Count - 1; i <= gen; i++) animalVar[11].Add(0);
            
            Debug.Log((animalMean[12].Count - 1) - gen);
            (float meanAge, float varAge) =
                GetNewMeanVariance(animalMean[12][gen], animalVar[12][gen], am.age, totalDeadAnimals[gen]);
            
            (float meanDist, float varDist) =
                GetNewMeanVariance(animalMean[11][gen], animalVar[11][gen], distanceTravelled, totalDeadAnimals[gen]);
                
            animalMean[12][gen] = meanAge;
            animalVar[12][gen] = varAge;
            
            animalMean[11][gen] = meanDist;
            animalVar[11][gen] = varDist;

            causeOfDeath[cause] = causeOfDeath[cause] += 1;
            Debug.Log(cause.ToString());
            
            switch (am)
            {
                case RabbitModel _ :
                    currentAnimalsTotalAlivePerSpecies[0] -= 1;
                    break;
                case WolfModel _ :
                    currentAnimalsTotalAlivePerSpecies[1] -= 1;
                    break;
                case DeerModel _  :
                    currentAnimalsTotalAlivePerSpecies[2] -= 1;
                    break;
                case BearModel _  :
                    currentAnimalsTotalAlivePerSpecies[3] -= 1;
                    break;
            }

            totalAnimalsAlive--;
            if(totalAnimalsAlive <= 0) onAllExtinct?.Invoke();
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
                classTraits.desirability,
                classTraits.viewAngle,
                classTraits.viewRadius,
                classTraits.hearingRadius,
            };
            return traits;
        }
        
        /// <summary>
        /// Not used, Check the entire scene for all objects with the "Animal" tag and add at the end of the time series list. 
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

        public void CollectNewFood()
        {
            if (foodActivePerMinute.Count - 1 < timeIndexFood) foodActivePerMinute.Add(1);
            else foodActivePerMinute[timeIndexFood] += 1;
        }
        public void CollectDeadFood()
        {
            foodActivePerMinute[timeIndexFood] -= 1;
        }
    }
}    