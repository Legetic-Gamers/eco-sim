using AnimalsV2;
using UnityEngine;

public class DummyAnimalController : AnimalController
{
    
    new void Awake()
    {
        base.Awake();
        animalModel = new RabbitModel();
        animalModel.reproductiveUrge = 100f;
        fsm.SetDefaultState(idleState);
        fsm.ChangeState(idleState);
        fsm.currentState.currentStateAnimation = StateAnimation.Idle;
    }
    public override void ChangeModifiers(State state)
    {
        //Do nothing
    }

    public override void UpdateParameters()
    {
        //do nothing
    }
    
    public override Vector3 getNormalizedScale()
    {
        return new Vector3(1f, 1f, 1f);
    }
}
