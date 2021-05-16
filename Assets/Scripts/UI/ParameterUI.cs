using System.Collections;
using System.Collections.Generic;
using Menus;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class ParameterUI : MonoBehaviour
{
    [SerializeField] private GameObject animal;
    [SerializeField] private Text state;
    //[SerializeField] private Slider health;
    [SerializeField] private Slider energy;
    [SerializeField] private Slider hydration;
    [SerializeField] private Slider reproductiveUrge;
    [SerializeField] private Slider age;
    
    [Header("Note disable -> lower performance")]
    [SerializeField] private bool onlyUpdateNearCamera;

    [HideInInspector]
    public Camera camera;
    
    private TickEventPublisher tickEventPublisher;
    private AnimalController animalController;

    private const float cameraDistanceThreshold = 50f;
    private Renderer renderer;

    private bool isLocked;
    
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
            LookAtCamera();
        }
    }


    void LookAtCamera()
    {
        if (camera && transform)
        {
            transform.LookAt(transform.position + camera.transform.rotation * Vector3.back, camera.transform.rotation * Vector3.up);
        }
    }

    bool isCloseToCamera()
    {
        if (camera && animal)
        {
            return Vector3.Distance(camera.transform.position, animal.transform.position) < cameraDistanceThreshold;
            
        }

        return true;
    }
    public void UpdateUI()
    {
        
        if (renderer && renderer.isVisible && !onlyUpdateNearCamera)
        {
            UpdateRenderParameterUI();
        }
        else if(renderer && renderer.isVisible && onlyUpdateNearCamera && isCloseToCamera())
        {
            UpdateRenderParameterUI();
        }

        if (OptionsMenu.alwaysShowParameterUI && !isLocked)
        {
            if (!isCloseToCamera() && gameObject.activeSelf) gameObject.SetActive(false);
            else if(isCloseToCamera() && !gameObject.activeSelf) gameObject.SetActive(true);     
        }
        
        
    }

    public void UpdateRenderParameterUI()
    {
        if (gameObject.activeSelf)
        {
            AnimalModel animal = animalController.animalModel;
            //health.value = animal.GetHealthPercentage;
            energy.value = animal.EnergyPercentage;
            hydration.value = animal.HydrationPercentage;
            reproductiveUrge.value = animal.ReproductiveUrgePercentage;
            age.value = animal.AgePercentage;
            if(animalController.fsm.currentState != null) state.text = animalController.fsm.currentState.ToString();    
        }
        
    }

    public void SetUIActive(bool value, bool isLocked = false)
    {
        this.isLocked = isLocked;

        gameObject.SetActive(value);
        if (value)
        {
            UpdateUI();
        }
        
    }
}
