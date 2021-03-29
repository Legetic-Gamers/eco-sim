using System.Collections;
using UnityEngine;

namespace AnimalsV2.States
{
    public class Waiting : State
    {

        private float waitTime = 1f;
        public Waiting(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
            currentStateAnimation = StateAnimation.Idle;
        }

        public override void Enter()
        {
            base.Enter();
            currentStateAnimation = StateAnimation.Idle;
            
            if (animal.agent.isActiveAndEnabled && animal.agent.isOnNavMesh)
            {
                animal.agent.isStopped = true;
            }

            //GetNearestFood();
            animal.StartCoroutine(Wait());
        }

        private IEnumerator Wait()
        {

            // Wait a while then change state and resume walking
            yield return new WaitForSeconds(waitTime/Time.timeScale);
            finiteStateMachine.GoToDefaultState();
            if (animal.agent.isActiveAndEnabled && animal.agent.isOnNavMesh)
            {
                animal.agent.isStopped = false;
            }

            // Very important, this tells Unity to move onto next frame. Everything crashes without this
            yield return null;
        }

        public override void Exit()
        {
            base.Exit();
            if (animal.agent.isActiveAndEnabled && animal.agent.isOnNavMesh)
            {
                animal.agent.isStopped = false;
            }
        }
        public override void HandleInput()
        {
            base.HandleInput();
            //Debug.Log("EXECUTING IDLE");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
        }

        public void SetWaitTime(float time)
        {
            this.waitTime = time;
        }

        public override string ToString()
        {
            return "Waiting";
        }

        public override bool MeetRequirements()
        {
            return true;
        }
    }
}