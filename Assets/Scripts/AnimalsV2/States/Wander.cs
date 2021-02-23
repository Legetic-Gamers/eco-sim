/*
 * Author: Johan A.
 */

using UnityEngine;

namespace AnimalsV2.States
{
//State where the animal just sits/ stands still.
//sealed just prevents other classes from inheriting
    public class Wander : State
    {
        public Wander(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine) {}

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Entered Search");
            currentStateAnimation = StateAnimation.Running;
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public  override void LogicUpdate()
        {
            base.LogicUpdate();
            if (animal.agent.remainingDistance < 1.0f)
            {
                Vector3 position = new Vector3(Random.Range(-20.0f, 20.0f), 0, Random.Range(-20.0f, 20.0f));
                animal.agent.SetDestination(position);
            }
        }
    }
}