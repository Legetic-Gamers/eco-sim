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

            System.Random rng = new System.Random();
            // Mate
            AnimalModel child = parent1.Mate(rng, parent2);
            
            // We make sure that parent1 is much likely to pass down the gense in comparison to parent2
            Debug.Log("Click on me to see traits after crossover"+ "\n" + 
                      " size: " + child.traits.size + "\n"
                      + "maxEnergy: " + child.traits.maxEnergy + "\n" 
                      + "maxHealth: " + child.traits.maxHealth + "\n"
                      + "maxHydration: " + child.traits.maxHydration + "\n" 
                      + "maxHydration: " + child.traits.maxHydration + "\n"
                      + "maxSpeed: " + child.traits.maxSpeed + "\n"
                      + "maxSpeed: " + child.traits.maxSpeed + "\n"
                      + "endurance: " + child.traits.endurance + "\n"
                      + "ageLimit: " + child.traits.ageLimit + "\n"
                      + "desirability: " + child.traits.desirability + "\n"
                      + "viewAngle: " + child.traits.viewAngle + "\n"
                      + "viewRadius: " + child.traits.viewRadius + "\n"
                      + "hearingRadius: " + child.traits.hearingRadius);
            
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