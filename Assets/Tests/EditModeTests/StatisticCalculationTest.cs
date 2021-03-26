using DataCollection;
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
    }
}