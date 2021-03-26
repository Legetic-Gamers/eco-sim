using DataCollection;
using Model;
using NUnit.Framework;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Tests.EditModeTests
{
    public class StatisticCalculationTest
    {
        
        AnimalModel am1 = new RabbitModel(new Traits(1f, 50, 100, 100, 6,1,
            10,100,10,10,180,5,3),0);
        AnimalModel am2 = new RabbitModel(new Traits(1f, 50, 100, 100, 6,1,
            10,100,10,10,180,5,3),0);
        AnimalModel am3 = new RabbitModel(new Traits(2f, 50, 100, 100, 6,1,
            10,100,10,10,180,5,3),0);
        AnimalModel am4 = new RabbitModel(new Traits(2f, 50, 100, 100, 6,1,
            10,100,10,10,180,5,3),1);
        AnimalModel am5 = new RabbitModel(new Traits(3f, 50, 100, 100, 6,1,
            10,100,10,10,180,5,3),1);

        private PlantModel pl1 = new PlantModel();
        private PlantModel pl2 = new PlantModel();
        private PlantModel pl3 = new PlantModel();
        private PlantModel pl4 = new PlantModel();
        private PlantModel pl5 = new PlantModel();
        /// <summary>
        /// Checks mean calculations of rabbit sizes. 
        /// </summary>
        [Test]
        public void CalculatesMeanSizePasses()
        {
            Collector c = new Collector();
            c.CollectBirth(am1);
            c.CollectBirth(am2);
            c.CollectBirth(am3);
            c.CollectBirth(am4);
            c.CollectBirth(am5);
            Assert.AreEqual(1.33333f, c.rabbitStatsPerGenMean[0][0], 0.00001f);
        }
        /// <summary>
        /// Checks variance calculation of rabbits. 
        /// </summary>
        [Test]
        public void CalculatesVarSizePasses()
        {
            Collector c = new Collector();
            c.CollectBirth(am1);
            c.CollectBirth(am2);
            c.CollectBirth(am3);
            c.CollectBirth(am4);
            c.CollectBirth(am5);
            Assert.AreEqual(0.333333f, c.rabbitStatsPerGenVar[0][0], 0.00001f);
        }
        /// <summary>
        /// Checks calculation of total rabbits.
        /// </summary>
        [Test]
        public void CalculatesTotalAnimalsPasses()
        {
            Collector c = new Collector();
            c.CollectBirth(am1);
            c.CollectBirth(am2);
            c.CollectBirth(am3);
            c.CollectBirth(am4);
            c.CollectBirth(am5);
            Assert.AreEqual(3f, c.rabbitTotalAlivePerGen[0]);
        }
        [Test]
        public void GenerationsWorking()
        {
            Collector c = new Collector();
            c.CollectBirth(am1);
            c.CollectBirth(am2);
            c.CollectBirth(am3);
            c.CollectBirth(am4);
            c.CollectBirth(am5);
            Assert.AreEqual(1.3333333f, c.rabbitStatsPerGenMean[0][0], 0.0001f);
            Assert.AreEqual(2.5, c.rabbitStatsPerGenMean[0][1], 0.0001f);
        }
        [Test]
        public void BirthRateWorking()
        {
            Collector c = new Collector();
            c.CollectBirth(am1);
            c.CollectBirth(am2);
            c.CollectBirth(am3);
            c.Collect();
            c.CollectBirth(am4);
            c.CollectBirth(am5);
            c.Collect();
            Assert.AreEqual(0.6666667f, c.birthRatePerMinute[0][1], 0.0001f);
        }
        [Test]
        public void TotalFoodPerMinuteWorking()
        {
            Collector c = new Collector();
            c.CollectNewFood(pl1);
            c.CollectNewFood(pl2);
            c.CollectNewFood(pl3);
            c.CollectNewFood(pl4);
            c.CollectNewFood(pl5);
            c.Collect();
            Assert.AreEqual(5f, c.foodActivePerMinute[0], 0.0001f);
            c.CollectDeadFood(pl1);
            c.CollectDeadFood(pl2);
            c.CollectDeadFood(pl3);
            c.Collect();
            Assert.AreEqual(2f, c.foodActivePerMinute[1], 0.0001f);
        }
        
        [Test]
        public void CauseOfDeathsWorking()
        {
            Collector c = new Collector();
            c.CollectDeath(am1, AnimalController.CauseOfDeath.Eaten);
            c.CollectDeath(am1, AnimalController.CauseOfDeath.Hydration);
            c.CollectDeath(am1, AnimalController.CauseOfDeath.Eaten);
            c.CollectDeath(am1, AnimalController.CauseOfDeath.Health);
            c.CollectDeath(am1, AnimalController.CauseOfDeath.Hunger);
            Assert.AreEqual(2, c.causeOfDeath[AnimalController.CauseOfDeath.Eaten]);
            Assert.AreEqual(1, c.causeOfDeath[AnimalController.CauseOfDeath.Hydration]);
            Assert.AreEqual(1, c.causeOfDeath[AnimalController.CauseOfDeath.Health]);
            Assert.AreEqual(1, c.causeOfDeath[AnimalController.CauseOfDeath.Hunger]);
        }
    }
}