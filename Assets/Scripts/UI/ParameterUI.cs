using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParameterUI : MonoBehaviour
{
    [SerializeField] private GameObject animal;
    [SerializeField] private Slider health;
    [SerializeField] private Slider energy;
    [SerializeField] private Slider satiety;
    [SerializeField] private Slider hydration;
    [SerializeField] private Slider reproductiveUrge;

    public Camera camera;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.back, camera.transform.rotation * Vector3.up);

        AnimalController animalController = animal.GetComponent<AnimalController>();
        if (animalController)
        {
            AnimalModel animal = animalController.animal;
            health.value = animal.currentHealth / animal.traits.maxHealth;
            energy.value = (float)animal.currentEnergy / animal.traits.maxEnergy;
            satiety.value = animal.satiety;
            hydration.value = animal.hydration;
            reproductiveUrge.value = animal.reproductiveUrge;
        }
    }
}
