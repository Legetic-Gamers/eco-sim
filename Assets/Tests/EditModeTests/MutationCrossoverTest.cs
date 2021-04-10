using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class MutationCrossoverTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void TestCrossover()
        {
            // Initialize two parents
            AnimalModel parent1 = new RabbitModel(new Traits(10,10,10,10, 10,1,10,10,10,10,10,10), 0);
            AnimalModel parent2 = new RabbitModel(new Traits(1,1,1,1, 1,1,1,1,1,1,1,1), 1);

            // Set age
            parent1.age = 80;
            parent2.age = 20;

            // Mate
            AnimalModel child = parent1.Mate(parent2);
            
            // We make sure that parent1 is much likely to pass down the gense in comparison to parent2
            DebugLogTraits(child.traits);
            
        }

        [Test]
        public void TestMutation()
        {
            AnimalModel animal1 = new RabbitModel(new Traits(100,100,100,100, 100,100,100,100,100,100,100,100), 1);

            animal1.traits.Mutation(1f);
            
            DebugLogTraits(animal1.traits);
        }


        public void DebugLogTraits(Traits traits)
        {
            // We make sure that parent1 is much likely to pass down the gense in comparison to parent2
            Debug.Log("Click on me to see traits"+ "\n" + 
                      " size: " + traits.size + "\n"
                      + "maxEnergy: " + traits.maxEnergy + "\n" 
                      + "maxHealth: " + traits.maxHealth + "\n"
                      + "maxHydration: " + traits.maxHydration + "\n"
                      + "maxSpeed: " + traits.maxSpeed + "\n"
                      + "acceleration: " + traits.acceleration + "\n"
                      + "endurance: " + traits.endurance + "\n"
                      + "ageLimit: " + traits.ageLimit + "\n"
                      + "desirability: " + traits.desirability + "\n"
                      + "viewAngle: " + traits.viewAngle + "\n"
                      + "viewRadius: " + traits.viewRadius + "\n"
                      + "hearingRadius: " + traits.hearingRadius);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}