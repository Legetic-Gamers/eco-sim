using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnimalsV2;
using AnimalsV2.States;
using AnimalsV2.States.AnimalsV2.States;
using Unity.Barracuda;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.AI;
using static AnimalsV2.Priorities;

public class AnimalBrainAgent : Agent
{
    //ANIMAL RELATED THINGS
    private AnimalController animalController;
    private AnimalModel animalModel;
    private TickEventPublisher eventPublisher;
    private FiniteStateMachine fsm;
    
    //
     private World world;


    //We could have multiple brains, EX:
    // Brain to use when no wall is present
    //public NNModel noWallBrain;
    // Brain to use when a jumpable wall is present
    //public NNModel smallWallBrain;
    // Brain to use when a wall requiring a block to jump over is present
    //public NNModel bigWallBrain;
    public void Start()
    {
        Debug.Log("Brain Awake");

        animalController = GetComponent<AnimalController>();
        fsm = animalController.fsm;
        animalModel = animalController.animalModel;
        world = FindObjectOfType<World>();
        
        EventSubscribe();
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();

        //Reset stuff 
        //MAKE SURE YOU ARE USING LOCAL POSITION

        //Reset animal position and rotation.
        transform.localPosition = new Vector3(Random.Range(-9.5f, 9.5f), 0, Random.Range(-9.5f, 9.5f));
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));

        world.ResetWorld();
        //transform.localPosition = new Vector3(Random.Range(-4.5f,0f),0,Random.Range(-3f,1.3f));
    }

    //Collecting observations that the ML agent should base its calculations on.
    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);

        //parameters of the Animal = 5
        //Right now these are continous, might need to be discrete (using lowEnergy() ex.) to represent conditions.
        sensor.AddObservation(animalModel.currentEnergy);
        sensor.AddObservation(animalModel.currentSpeed);
        sensor.AddObservation(animalModel.currentHydration);
        sensor.AddObservation(animalModel.currentHealth);
        sensor.AddObservation(animalModel.reproductiveUrge);

        
    }

    //Called Every time the ML agent decides to take an action.
    public override void OnActionReceived(float[] vectorAction)
    {
        base.OnActionReceived(vectorAction);

        PerformBestAction(vectorAction);
    }

    //Used for testing, gives us control over the output from the ML algortihm.
    public override void Heuristic(float[] actionsOut)
    {
        
    }

    /// <summary>
    /// Set constraints based on current State.
    /// 
    /// </summary>
    /// <returns></returns>
    private void PerformBestAction(float[] vectorAction)
    {
        

    }

    private void ChangeState(State newState)
    {
        //Debug.Log(newState.ToString());
        fsm.ChangeState(newState);
    }

    //Instead of updating/Making choices every frame
    //Listen to when parameters or senses were updated.
    private void EventSubscribe()
    {
        // eventPublisher.onParamTickEvent += MakeDecision;
        // eventPublisher.onSenseTickEvent += MakeDecision;

        animalController.actionDeath += HandleDeath;
    }


    public void EventUnsubscribe()
    {
        // eventPublisher.onParamTickEvent -= MakeDecision;
        // eventPublisher.onSenseTickEvent -= MakeDecision;

        animalController.actionDeath -= HandleDeath;
    }

    /// <summary>
    /// Handle perceived target such that GetBestAction can then use it in deciding an action.
    /// </summary>
    /// <param name="target"> perceived target sent from either FieldOfView or HearingAbility,
    /// which method that will be called depends on the type of target </param>


    //To set reward we could use biological fitness = fertile ofspring produced = Good reward
    //addreward offspringConstant (could be like 20, depends on age)
    //This should be added on reproduce? so need event for that?
    
    //Penalty could be set upon death. Perhaps based on how many fertile offspring produced in lifetime and
    //how long the rabbit lived?

    private void HandleDeath()
    {
        int offspringConstant = 20;
        
        //Penalize for every year not lived.
        AddReward( animalModel.age - animalModel.traits.ageLimit );
        ChangeState(animalController.deadState);
        
        EndEpisode();
        

        EventUnsubscribe();
    }
    
    
    public void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            print("up");
            Vector3 position = gameObject.transform.position;
            position.x = position.x + 1f;
            NavMeshHit hit;
            NavMesh.SamplePosition(position, out hit, 5, 1 << NavMesh.GetAreaFromName("Walkable"));
            animalController.agent.SetDestination(hit.position);
        } else if (Input.GetKey(KeyCode.DownArrow))
        {
            Vector3 position = gameObject.transform.position;
            position.x = position.x - 1f;
            NavMeshHit hit;
            NavMesh.SamplePosition(position, out hit, 5, 1 << NavMesh.GetAreaFromName("Walkable"));
            animalController.agent.SetDestination(hit.position);
            print("down");
        } else if (Input.GetKey(KeyCode.LeftArrow))
        {
            Vector3 position = gameObject.transform.position;
            position.z = position.z + 1f;
            NavMeshHit hit;
            NavMesh.SamplePosition(position, out hit, 5, 1 << NavMesh.GetAreaFromName("Walkable"));
            animalController.agent.SetDestination(hit.position);
            print("left");
        } else if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector3 position = gameObject.transform.position;
            position.z = position.z - 1f;
            NavMeshHit hit;
            NavMesh.SamplePosition(position, out hit, 5, 1 << NavMesh.GetAreaFromName("Walkable"));
            animalController.agent.SetDestination(hit.position);
            print("right");
        } else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeState(animalController.goToFoodState);
            print("1");
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeState(animalController.goToWaterState);
            print("2");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeState(animalController.wanderState);
            print("3");
        } else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeState(animalController.fleeingState);
            print("4");
        }
    }
}