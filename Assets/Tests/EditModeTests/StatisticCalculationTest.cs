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
            10,100,10,10,180,5,3),0);
        AnimalModel am5 = new RabbitModel(new Traits(3f, 50, 100, 100, 6,1,
            10,100,10,10,180,5,3),0);
        
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
            Assert.AreEqual(1.8f, c.rabbitStatsPerGenMean[0][0], 0.00001f);
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
            Assert.AreEqual(0.7f, c.rabbitStatsPerGenVar[0][0], 0.00001f);
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
            Assert.AreEqual(5f, c.rabbitTotalAlivePerGen[0]);
        }
    }
}