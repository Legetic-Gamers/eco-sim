using UnityEngine;

namespace AnimalsV2.States
{

    namespace AnimalsV2.States
    {
        public class Dead : State
        {

            public Dead(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
            {
                
            }

            public override void Enter()
            {
                //when entering state dead,

                Debug.Log("Dead");
                base.Enter();
                animal.agent.isStopped = true;
                currentStateAnimation = StateAnimation.Dead;
                animal.DestroyGameObject(20f);
            }

            public override void HandleInput()
            {
                base.HandleInput();
            
            }

            public override void LogicUpdate()
            {
                base.LogicUpdate();
             
            }

            public override string ToString()
            {
                return "Dead";
            }

            public override bool MeetRequirements()
            {
                return true;
            }
            
        }
    }
}