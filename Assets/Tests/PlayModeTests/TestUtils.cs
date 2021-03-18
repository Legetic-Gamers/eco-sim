using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Tests
{
    public class TestUtils
    {
        public class TestWorld
        {
            public GameObject animal;
            public GameObject smartAnimal;
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

            
                animal = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Animals/Rabbit Brown.prefab");
                animal.GetComponent<NavMeshAgent>().enabled = false;
                animal = GameObject.Instantiate(animal, new Vector3(0, 0, 10), new Quaternion(0, 180, 0, 0));
                
                //
                smartAnimal = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scripts/SmartBehavior/SmartAnimal/SmartRabbit.prefab");
                smartAnimal.GetComponent<NavMeshAgent>().enabled = false;
                smartAnimal = GameObject.Instantiate(smartAnimal, new Vector3(0, 0, 10), new Quaternion(0, 180, 0, 0));

            }
        }
    }
}