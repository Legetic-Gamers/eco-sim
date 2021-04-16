using System;
using System.Collections;
using System.Linq;
using AnimalsV2;
using AnimalsV2.States;
using AnimalsV2.States.AnimalsV2.States;
using Model;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.AI;
using ViewController;
using ViewController.Senses;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(MLAnimalController)), RequireComponent(typeof(Senses))]
public class DumbAgent : Agent, IAgent
{
    //ANIMAL RELATED THINGS
    private MLAnimalController animalController;
    private AnimalModel animalModel;
    private Senses senses;
    private FiniteStateMachine fsm;
    public Action<float> onEpisodeBegin { get; set; }
    public Action<float> onEpisodeEnd { get; set; }

    
    public void Awake()
    {
        //make sure sensetick is using constant tick interval
        senses = GetComponent<Senses>();
        senses.useConstantTickInterval = true;
        
        //init specific
        animalController = GetComponent<MLAnimalController>();
        if(animalController) animalController.OnStartML += Init;
    }

    private void Init()
    {
        animalModel = animalController.animalModel;
        fsm = animalController.fsm;
        
        //set infertile if training
        animalController.isInfertile = animalController.isTraining;
        if(animalController.isInfertile) Debug.Log("Agent is infertile!");
        
        EventSubscribe();
    }
    
    public override void OnEpisodeBegin()
    {
        onEpisodeBegin?.Invoke(100f);
    }
    

    //Collecting observations that the ML agent should base its calculations on.
    //Choices based on https://github.com/Unity-Technologies/ml-agents/blob/release_2_verified_docs/docs/Learning-Environment-Design-Agents.md#vector-observations
    public override void CollectObservations(VectorSensor sensor)
    {
        if (animalModel == null || animalController == null) return;
        //Position of the animal
        Vector3 thisPosition = transform.position;
        //Get the absolute vector for all targets
        Vector3 nearestFood = NavigationUtilities.GetNearestObject(animalController.visibleFoodTargets.Concat(animalController.heardPreyTargets).ToList(), thisPosition)?.transform.position ?? thisPosition;
        Vector3 nearestWater = NavigationUtilities.GetNearestObject(animalController.visibleWaterTargets, thisPosition)?.transform.position ?? thisPosition;
        //Get the absolute vector for a potential mate
        Vector3 potentialMate = animalController?.goToMate.GetFoundMate()?.transform.position ?? thisPosition;
        
        //Get Vector between animal and targets
        nearestFood = transform.InverseTransformPoint(nearestFood);
        nearestWater = transform.InverseTransformPoint(nearestWater);
        potentialMate = transform.InverseTransformPoint(potentialMate);

        //Get the magnitude of nearestFood, nearestWater potentialMate. (Normalized)
        float maxPercievableDistance = animalController.animalModel.traits.viewRadius;
        float nearestFoodDistance = nearestFood.magnitude / maxPercievableDistance;
        float nearestWaterDistance = nearestWater.magnitude / maxPercievableDistance;
        float potentialMateDistance = potentialMate.magnitude / maxPercievableDistance;

        //normalize some of the observations
        if (nearestFood != Vector3.zero) nearestFood = nearestFood.normalized;
        if (nearestWater != Vector3.zero) nearestWater = nearestWater.normalized;
        if (potentialMate != Vector3.zero) potentialMate = potentialMate.normalized;
        
        //Add observations for food
        sensor.AddObservation(nearestFoodDistance);
        sensor.AddObservation(nearestFood.x);
        sensor.AddObservation(nearestFood.z);
        sensor.AddObservation(animalModel.GetEnergyPercentage);
        
        //Add observations for water
        sensor.AddObservation(nearestWaterDistance);
        sensor.AddObservation(nearestWater.x);
        sensor.AddObservation(nearestWater.z);
        sensor.AddObservation(animalModel.GetHydrationPercentage);
        
        //Add observations for mate
        sensor.AddObservation(potentialMateDistance);
        sensor.AddObservation(potentialMate.x);
        sensor.AddObservation(potentialMate.z);
        sensor.AddObservation(animalModel.WantingOffspring);
        
        
        //Add agents velocity (as a direction) to observations
        //Vector3 velocity = transform.InverseTransformDirection(animalController.agent.velocity);
        
        Vector3 velocity = transform.InverseTransformVector(animalController.agent.velocity);
        sensor.AddObservation(velocity.x);
        sensor.AddObservation(velocity.z);
    }

    //Called Every time the ML agent decides to take an action.
    //steering inspired by: https://github.com/Unity-Technologies/ml-agents/blob/release_2_verified_docs/Project/Assets/ML-Agents/Examples/FoodCollector/Scripts/FoodCollectorAgent.cs
    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 dirToGo = transform.forward;
        
        //Continuous actions are preclamped by mlagents [-1, 1]
        float rotationAngle = actions.ContinuousActions[0] * 110; //90
        
        //Give reward based on how much rotation is made. 0 is no rotation and 1 (or -1) is max rotation.
        //positive reward is better for shaping desired behavior
        AddReward(0.005f - Math.Abs(actions.ContinuousActions[0]) * 0.005f);
        
        //Rotate vector based on rotation from the action
        dirToGo = Quaternion.AngleAxis(rotationAngle, Vector3.up) * dirToGo;
        dirToGo *= 4;

        float speedModifier = actions.ContinuousActions[1];
        speedModifier = 0.5f * speedModifier + 0.5f; //make sure that function of interval [-1,1] maps to [0,1]
        
        //Give reward for moving forward
        AddReward(speedModifier * 0.005f);

        //Set speed
        animalController.SetSpeed(speedModifier);
        
        NavigationUtilities.NavigateRelative(animalController, dirToGo, 1 << NavMesh.GetAreaFromName("Walkable"));
    }

    //Used for testing, gives us control over the output from the ML algortihm.
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //represents rotation of -90 degrees
            continuousActions[0] = -0.5f;
        } else if (Input.GetKey(KeyCode.RightArrow))
        {
            //represents rotation of 90 degrees
            continuousActions[0] = 0.5f;
        }
        else
        {
            continuousActions[0] = 0;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            continuousActions[1] = 1;
        }
        else
        {
            continuousActions[1] = -1;
        }
        

    }
    
    
    private void HandleDeath(AnimalController animalController, bool gotEaten)
    {
        AddReward(-1);
        onEpisodeEnd?.Invoke(100f);
        //Task failed
        EndEpisode();
    }
    
    private void HandleDrink(GameObject water, float currentHydration)
    {
        // the reward should be proportional to how much hydration was gained when drinking
        float reward = animalModel.traits.maxHydration - currentHydration;
        // normalize reward as a percentage
        reward /= animalModel.traits.maxHydration;
        //AddReward(reward * 0.1f);
        AddReward(reward * 0.1f);
    }

    //The reason to why I have curentEnergy as an in-parameter is because currentEnergy is updated through EatFood before reward gets computed in AnimalMovementBrain
    private void HandleEat(GameObject food, float currentEnergy)
    {
        //Debug.Log("currentEnergy: " + animalController.animalModel.currentEnergy);
        float reward = 0f;
        //Give reward
        if (food.GetComponent<AnimalController>()?.animalModel is IEdible edibleAnimal &&
            animalModel.CanEat(edibleAnimal))
        {
            float nutritionReward = edibleAnimal.nutritionValue;
            float hunger = animalModel.traits.maxEnergy - currentEnergy;

            //the reward for eating something should be the minimum of the actual nutrition gain and the hunger. Reason is that if an animal eats something that when it is already satisfied it will return a low reward.
            reward = Math.Min(nutritionReward, hunger);
            // normalize reward as a percentage
            reward /= animalModel.traits.maxEnergy;
        } else if (food.GetComponent<PlantController>()?.plantModel is IEdible ediblePlant && animalModel.CanEat(ediblePlant))
        {
            float nutritionReward = ediblePlant.nutritionValue;
            float hunger = animalModel.traits.maxEnergy - currentEnergy;

            //the reward for eating something should be the minimum of the actual nutrition gain and the hunger. Reason is that if an animal eats something that when it is already satisfied it will return a low reward.
            reward = Math.Min(nutritionReward, hunger);
            reward /= animalModel.traits.maxEnergy;
        }

        AddReward(reward * 0.1f);
    }
    
    private void HandleMate(GameObject obj)
    {
        //SetReward(1f);
        AddReward(2f);
        //Task achieved
        //Task achieved
        onEpisodeEnd?.Invoke(100f);
        EndEpisode();
    }

    private void PreRequestDecision()
    {
        //make sure no decision is taken when a blocking state is running
        if (!(fsm.currentState is EatingState) && !(fsm.currentState is DrinkingState) &&
            !(fsm.currentState is MatingState) && !(fsm.currentState is FleeingState) && animalController != null && !(fsm.currentState is Dead))
        {
            RequestDecision();
        }
    }
    
    //Listen to when parameters or senses were updated.
    private void EventSubscribe()
    {
        StartCoroutine(SubscribeTickEvent());

        if (animalController)
        {
            animalController.deadState.onDeath += HandleDeath;
            animalController.eatingState.onEatFood += HandleEat;
            animalController.matingState.onMate += HandleMate;
            animalController.drinkingState.onDrinkWater += HandleDrink;
            animalController.actionPerceivedHostile += HandleHostileTarget;    
        }
    }

    //method used to spread out when ticks happen, this is pretty crucial for performance in real simulation. 
    IEnumerator SubscribeTickEvent()
    {
        float delaySeconds = Random.Range(0, 0.5f);

        if (animalController.isTraining)
        {
            Debug.Log("No delay on subscribing tick event");
            delaySeconds = 0;
        }
        
        // Wait a while then change state and resume walking
        yield return new WaitForSeconds(delaySeconds/Time.timeScale);
        
        //subscribe to own onSenseTick
        if (senses)
        {
            senses.onSenseTick += PreRequestDecision;
        }
    }


    public void EventUnsubscribe()
    {
        if (senses)
        {
            senses.onSenseTick -= PreRequestDecision;
        }

        if (animalController)
        {
            animalController.deadState.onDeath -= HandleDeath;
            animalController.eatingState.onEatFood -= HandleEat;
            animalController.matingState.onMate -= HandleMate;
            animalController.drinkingState.onDrinkWater -= HandleDrink;
            animalController.actionPerceivedHostile -= HandleHostileTarget;    
        }
    }

    private void OnDestroy()
    {
        if(animalController) animalController.OnStartML -= Init;
        EventUnsubscribe();
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Target"))
        {
            Interact(other.gameObject);
        }
    }
    
    private void HandleHostileTarget(GameObject target)
    {
        animalController.SetSpeed(animalController.speedModifier);
        fsm.ChangeState(animalController.fleeingState);
    }
    
    //General method that takes unknown gameobject as input and interacts with the given gameobject depending on what it is. It can be to e.g. consume or to mate
    // It is not guaranteed that the statechange will happen since meetrequirements has to be true for given statechange.
    public void Interact(GameObject target)
    {
        //Dont start to interact if we are fleeing.
        if (fsm == null || fsm.currentState is FleeingState || fsm.currentState is Dead) return;

        //Debug.Log(gameObject.name);
        switch (target.tag)
        {
            case "Water":
                animalController.drinkingState.SetTarget(target);
                fsm.ChangeState(animalController.drinkingState);
                break;
            case "Plant":
                if (target.TryGetComponent(out PlantController plantController) &&
                    animalModel.CanEat(plantController.plantModel))
                {
                    animalController.eatingState.SetTarget(target);
                    fsm.ChangeState(animalController.eatingState);
                }

                break;
            case "Animal":
                if (target.TryGetComponent(out AnimalController otherAnimalController))
                {
                    AnimalModel otherAnimalModel = otherAnimalController.animalModel;
                    //if we can eat the other animal we try to do so
                    if (animalModel.CanEat(otherAnimalModel))
                    {
                        animalController.eatingState.SetTarget(target);
                        fsm.ChangeState(animalController.eatingState);
                    }
                    else if (animalModel.IsSameSpecies(otherAnimalModel))
                    {
                        animalController.matingState.SetTarget(target);
                        fsm.ChangeState(animalController.matingState);
                    }
                }

                break;
        }

    }
    
}