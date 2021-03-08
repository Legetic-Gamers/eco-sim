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
        public IEnumerator FSM_Configure_Passes()
        {
            // Wait for three seconds (this gives us time to see the prefab in the scene if its an animation or something else).
            //yield return new WaitForSeconds(3f);

            TestWorld testWorld = new TestWorld();

            GameObject animal = testWorld.animal;
            //var animal = NewAnimal();
            AnimalController animalController = animal.GetComponent<AnimalController>();
            FiniteStateMachine fsm = animalController.fsm;
            Assert.IsTrue(fsm.CurrentState is Wander);
            Assert.IsFalse(fsm.absorbingState);


            yield return null;
        }

        

        private class TestWorld
        {
            public GameObject animal;
            public GameObject eventPublisher;
            
            public TestWorld()
            {
                //Add camera to scene.
                RenderSettings.skybox = null;
                var root = new GameObject();
                root.AddComponent(typeof(Camera));
                var camera = root.GetComponent<Camera>();
                camera.tag = "MainCamera";
                root = GameObject.Instantiate(root);
            
                //Add tickEventPublisher to scene.
                eventPublisher = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/TickEventPublisher.prefab");
                eventPublisher = GameObject.Instantiate(eventPublisher, new Vector3(0, 0, 10), new Quaternion(0, 180, 0, 0));

            
                animal = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Rabbit Brown.prefab");
                animal.GetComponent<NavMeshAgent>().enabled = false;
                animal = GameObject.Instantiate(animal, new Vector3(0, 0, 10), new Quaternion(0, 180, 0, 0));

            }
        }

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


// 1
        // [UnityTest]
        // public IEnumerator AsteroidsMoveDown()
        // {
        //     // 2
        //     GameObject gameGameObject = 
        //         MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Game"));
        //     game = gameGameObject.GetComponent<Game>();
        //     // 3
        //     GameObject asteroid = game.GetSpawner().SpawnAsteroid();
        //     // 4
        //     float initialYPos = asteroid.transform.position.y;
        //     // 5
        //     yield return new WaitForSeconds(0.1f);
        //     // 6
        //     Assert.Less(asteroid.transform.position.y, initialYPos);
        //     // 7
        //     Object.Destroy(game.gameObject);
        // }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator FSMTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}