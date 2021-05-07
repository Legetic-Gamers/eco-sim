using AnimalsV2;
using AnimalsV2.States;
using UnityEngine;
using UnityEngine.AI;

public class DummyRabbitController : AnimalController
{
    
    new void Awake()
    {
        animalModel = new RabbitModel();
        base.Awake();
        animalModel.currentHydration = animalModel.traits.maxHydration;
        animalModel.currentEnergy = animalModel.traits.maxEnergy;
        animalModel.reproductiveUrge = 100f;
        
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = true;
        //Can be used later.
        baseAngularSpeed = agent.angularSpeed;
        baseAcceleration = agent.acceleration;

        animationController = GetComponent<AnimationController>();

        wanderState = new Wander2(this,fsm);
        
        fsm.SetDefaultState(wanderState);
        fsm.ChangeState(wanderState);
    }

    public override void onObjectSpawn()
    {
        //Do nothing
        agent.acceleration = baseAcceleration;
        agent.angularSpeed = baseAngularSpeed;
        StartCoroutine(UpdateStatesLogicLoop());

    }

    public override void ChangeModifiers(State state)
    {
        //Do nothing
    }

    public override void UpdateParameters()
    {
        //do nothing
    }

    protected override void SetPhenotype()
    {
        //do nothing
    }

    public override Vector3 getNormalizedScale()
    {
        return new Vector3(1f, 1f, 1f);
    }
    
    public override string GetObjectLabel()
    {
        return "DummyRabbit";
    }
}
