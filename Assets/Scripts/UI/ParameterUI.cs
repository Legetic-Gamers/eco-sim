using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParameterUI : MonoBehaviour
{
    [SerializeField] private GameObject animal;
    [SerializeField] private Text state;
    [SerializeField] private Slider health;
    [SerializeField] private Slider energy;
    [SerializeField] private Slider hydration;
    [SerializeField] private Slider reproductiveUrge;

    [HideInInspector]
    public Camera camera;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.back, camera.transform.rotation * Vector3.up);

        AnimalController animalController = animal.GetComponent<AnimalController>();
        if (animalController)
        {
            AnimalModel animal = animalController.animalModel;
            health.value = animal.GetHealthPercentage;
            energy.value = animal.GetEnergyPercentage;
            hydration.value = animal.GetHydrationPercentage;
            reproductiveUrge.value = animal.reproductiveUrge;
            state.text = animalController.Fsm.CurrentState.ToString();
        }
    }
}
