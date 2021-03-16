using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AnimalsV2;
using NUnit.Framework;
using UnityEngine.TestTools;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Tests
{
    public class AnimalBrainAgentTest
    {
        [Test]
        public void AnimalBrainAgentTestSimplePasses()
        {
            // Use the Assert class to test conditions.
        }

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityTest]
        public IEnumerator AnimalBrainAgentTest_Setup_Passes()
        {
            TestUtils.TestWorld testWorld = new TestUtils.TestWorld();

            AnimalBrainAgent animalBrainAgent = testWorld.smartAnimal.GetComponent<AnimalBrainAgent>();
            //HearingAbility hearingAbility = testWorld.smartAnimal.GetComponent<HearingAbility>();

           // Assert.NotNull(hearingAbility);
            Assert.NotNull(animalBrainAgent);


            // Component[] allComponents = testWorld.smartAnimal.GetComponentsInChildren<Component>();
            // foreach (var c in allComponents)
            // {
            //     Debug.Log(c.name);
            // }


            // testWorld.animal
            yield return null;
        }

        [UnityTest]
        public IEnumerator AnimalBrainAgentTest_Observation_Passes()
        {
            TestUtils.TestWorld testWorld = new TestUtils.TestWorld();

            AnimalBrainAgent animalBrainAgent = testWorld.smartAnimal.GetComponent<AnimalBrainAgent>();


            // VectorSensor testVectorSensor = new VectorSensor(7);
            // Debug.Log(testVectorSensor.GetObservationShape());
            ReadOnlyCollection<float> initialObservations = animalBrainAgent.GetObservations();
            float[] o = initialObservations.ToArray();
            yield return new WaitForSeconds(1.5f);
            ReadOnlyCollection<float> observations = animalBrainAgent.GetObservations();
            float[] o1 = observations.ToArray();
            yield return new WaitForSeconds(1f);
            ReadOnlyCollection<float> newObservations = animalBrainAgent.GetObservations();
            float[] o2 = newObservations.ToArray();

            CollectionAssert.AreNotEqual(o, o1,
                "AreNotEqual: " + o + " equal to " + o2 + ", Observations were not made.");
            CollectionAssert.AreNotEqual(o1, o2,
                "AreNotEqual: " + o1 + " equal to " + o2 + ", Observations were not made.");
            //animalBrainAgent.CollectObservations();

            // foreach (var f in o)
            // {
            //     Debug.Log(f);
            // }
            //
            // Debug.Log("------------");
            //
            // foreach (var f in o1)
            // {
            //     Debug.Log(f);
            // }
            //
            // Debug.Log("------------");
            // foreach (var f in o2)
            // {
            //     Debug.Log(f);
            // }
            // testWorld.animal
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator AnimalBrainAgentTest_Action_Passes()
        {
            TestUtils.TestWorld testWorld = new TestUtils.TestWorld();

            AnimalBrainAgent animalBrainAgent = testWorld.smartAnimal.GetComponent<AnimalBrainAgent>();
            AnimalController animalController = testWorld.smartAnimal.GetComponent<AnimalController>();
            FiniteStateMachine fsm = animalController.fsm;
            State stateToSwitchTo = fsm.CurrentState;
            
            
            ActionBuffers storedActionBuffers = animalBrainAgent.GetStoredActionBuffers();
            ActionSegment<int> discreteActions = storedActionBuffers.DiscreteActions;
            
            animalBrainAgent.RequestAction();
            switch (discreteActions[0])
            {
                case 0:
                    stateToSwitchTo = animalController.wanderState;
                    break;
                case 1:
                    stateToSwitchTo = animalController.goToWaterState;
                    break;
                case 2:
                    stateToSwitchTo = animalController.goToMate;
                    break;
                case 3:
                    stateToSwitchTo = animalController.goToWaterState;
                    break;
                case 4:
                    stateToSwitchTo = animalController.fleeingState;
                    break;
            }

            if (!(stateToSwitchTo == fsm.CurrentState || fsm.absorbingState || !stateToSwitchTo.MeetRequirements()))
            {
                //Assert that we actually switched state. Successful switch
                Assert.Equals(stateToSwitchTo, fsm.CurrentState);
            }
            
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator AnimalBrainAgentTest_Reward_Passes()
        {
            TestUtils.TestWorld testWorld = new TestUtils.TestWorld();

            AnimalBrainAgent animalBrainAgent = testWorld.smartAnimal.GetComponent<AnimalBrainAgent>();

            float oldReward = animalBrainAgent.GetCumulativeReward();
            yield return new WaitForSeconds(1f);
            float newReward = animalBrainAgent.GetCumulativeReward();

            //Make sure rewards have changed so that the hunger/thirst reward feature works.
            Assert.AreNotEqual(oldReward,newReward);
            // animalBrainAgent.GetCumulativeReward()
            yield return null;
        }
    }
}