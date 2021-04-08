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
    
    [Header("Note disable -> lower performance")]
    [SerializeField] private bool onlyUpdateNearCamera;

    [HideInInspector]
    public Camera camera;
    

    private TickEventPublisher tickEventPublisher;
    private AnimalController animalController;

    private const float cameraDistanceThreshold = 50f;
    private Renderer renderer;
    
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        renderer = animal.GetComponentInChildren<SkinnedMeshRenderer>();
        tickEventPublisher = FindObjectOfType<global::TickEventPublisher>();
        animalController = animal.GetComponent<AnimalController>();
        if (tickEventPublisher)
        {
            tickEventPublisher.onSenseTickEvent += UpdateUI;
        }
    }

    private void OnDestroy()
    {
        if (tickEventPublisher)
        {
            tickEventPublisher.onSenseTickEvent -= UpdateUI;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        // NOTE: this will be true even if the renderer is visible from scene view.
        if (renderer.isVisible)
        {
            lookAtCamera();
        }
    }


    void lookAtCamera()
    {
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.back, camera.transform.rotation * Vector3.up);
    }

    bool isCloseToCamera()
    {
        return Vector3.Distance(camera.transform.position, animal.transform.position) < cameraDistanceThreshold;
    }
    void UpdateUI()
    {
        
        if (renderer.isVisible && !onlyUpdateNearCamera)
        {
            UpdateRenderParameterUI();
        }
        else if(renderer.isVisible && onlyUpdateNearCamera && isCloseToCamera())
        {
            UpdateRenderParameterUI();
        }
    }

    void UpdateRenderParameterUI()
    {
        AnimalModel animal = animalController.animalModel;
        health.value = animal.GetHealthPercentage;
        energy.value = animal.GetEnergyPercentage;
        hydration.value = animal.GetHydrationPercentage;
        reproductiveUrge.value = animal.GetUrgePercentage;
        state.text = animalController.fsm.currentState.ToString();
    }
}
