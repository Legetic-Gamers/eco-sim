using System.Collections;
using System.Collections.Generic;
using AnimalsV2;
using AnimalsV2.States;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;

namespace Tests
{
    public class FSMTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void FSMTestSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // Testing the basic configuration of the FSM
        [UnityTest]
        public IEnumerator FSM_Setup_Passes()
        {

            //Setup
            TestUtils.TestWorld testWorld = new TestUtils.TestWorld();
            GameObject animal = testWorld.animal;
            AnimalController animalController = animal.GetComponent<AnimalController>();
            FiniteStateMachine fsm = animalController.fsm;
            
            //Check that initial state is correct.
            Assert.NotNull(fsm);
            if (fsm != null)
            {
                Assert.IsTrue(fsm.currentState is Wander);
                Assert.IsFalse(fsm.absorbingState);
            }


            //yield return new WaitForSeconds(3f);
            yield return null;
        }
        
        // Testing going to default state of the FSM
        [UnityTest]
        public IEnumerator FSM_DefaultState_Passes()
        {

            //Setup
            TestUtils.TestWorld testWorld = new TestUtils.TestWorld();
            GameObject animal = testWorld.animal;
            AnimalController animalController = animal.GetComponent<AnimalController>();
            FiniteStateMachine fsm = animalController.fsm;
            
            //Check that initial state is correct.
            Assert.IsTrue(fsm.currentState is Wander);
            fsm.ChangeState(animalController.idleState);
            fsm.GoToDefaultState();
            Assert.IsTrue(fsm.currentState is Wander);

            //yield return new WaitForSeconds(3f);
            yield return null;
        }

        
        // Testing the basic functionality of the FSM
        [UnityTest]
        public IEnumerator FSM_Requirements_Passes()
        {

            //Setup
            TestUtils.TestWorld testWorld = new TestUtils.TestWorld();
            GameObject animal = testWorld.animal;
            AnimalController animalController = animal.GetComponent<AnimalController>();
            FiniteStateMachine fsm = animalController.fsm;
            
            
            //State changing, make sure requirements checking works.
            if (animalController.goToFoodState.MeetRequirements())
            {
                fsm.ChangeState(animalController.goToFoodState);
                Assert.IsTrue(fsm.currentState is GoToFood);
            }
            else
            {
                Assert.IsFalse(fsm.currentState is GoToFood);
            }
            
            if (animalController.goToMate.MeetRequirements())
            {
                fsm.ChangeState(animalController.goToMate);
                Assert.IsTrue(fsm.currentState is GoToMate);
            }
            else
            {
                Assert.IsFalse(fsm.currentState is GoToMate);
            }
            
            if (animalController.goToWaterState.MeetRequirements())
            {
                fsm.ChangeState(animalController.goToWaterState);
                Assert.IsTrue(fsm.currentState is GoToWater);
            }
            else
            {
                Assert.IsFalse(fsm.currentState is GoToWater);
            }



            //yield return new WaitForSeconds(3f);
            yield return null;
        }

        

        // private class TestWorld
        // {
        //     public GameObject animal;
        //     public GameObject eventPublisher;
        //     
        //     public TestWorld()
        //     {
        //         //Add camera to scene.
        //         RenderSettings.skybox = null;
        //         var root = new GameObject();
        //         root.AddComponent(typeof(Camera));
        //         var camera = root.GetComponent<Camera>();
        //         camera.tag = "MainCamera";
        //         root = GameObject.Instantiate(root);
        //     
        //         //Add tickEventPublisher to scene.
        //         eventPublisher = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/TickEventPublisher.prefab");
        //         eventPublisher = GameObject.Instantiate(eventPublisher, new Vector3(0, 0, 10), new Quaternion(0, 180, 0, 0));
        //
        //     
        //         animal = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Rabbit Brown.prefab");
        //         animal.GetComponent<NavMeshAgent>().enabled = false;
        //         animal = GameObject.Instantiate(animal, new Vector3(0, 0, 10), new Quaternion(0, 180, 0, 0));
        //
        //     }
        // }

        // private GameObject NewAnimal()
        // {
        //     GameObject animal = new GameObject();
        //     animal.AddComponent(typeof(Animator));
        //     Animator animator = animal.GetComponent<Animator>();
        //     animator.runtimeAnimatorController = Resources.Load("Assets/Scripts/RabbitV2") as RuntimeAnimatorController;
        //     
        //     animal.AddComponent(typeof(FieldOfView));
        //     animal.AddComponent(typeof(NavMeshAgent));
        //     NavMeshAgent navMeshAgent = animal.GetComponent<NavMeshAgent>();
        //     navMeshAgent
        //     
        //     animal.AddComponent(typeof(HearingAbility));
        //     animal.AddComponent(typeof(RabbitController));
        //     animal.AddComponent(typeof(DecisionMaker));
        //     
        //
        //     return animal;
        // }


    }
}