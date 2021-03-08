using System.Collections;
using AnimalsV2;
using AnimalsV2.States;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;

namespace Tests
{
    public class DecisionMakerTest
    {
        
        // Testing going to default state of the FSM
        [UnityTest]
        public IEnumerator DecisionMaker_Nothing_Passes()
        {

            //yield return new WaitForSeconds(3f);
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
    }
}