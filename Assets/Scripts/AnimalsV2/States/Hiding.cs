using System.Collections;
using UnityEngine;
using ViewController;

namespace AnimalsV2.States
{
    public class Hiding : State
    {

        private HideoutController target;
        private Vector3 lastPosition;
        private bool isExiting;
        
        public Hiding(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
        }

        public override string ToString()
        {
            return "Hiding";
        }

        public override void Enter()
        {
            Debug.Log("HEHEH");
            base.Enter();
            target.EnterHideout();
            animal.agent.isStopped = true;
            lastPosition = animal.transform.position;
            animal.transform.position = target.transform.position;
            finiteStateMachine.absorbingState = true;
            isExiting = false;
        }

        public override void Exit()
        {
            base.Exit();
            animal.agent.isStopped = false;

            target.ExitHideout();
            target = null;
            animal.transform.position = lastPosition;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (animal.heardHostileTargets.Count == 0 && !isExiting)
            { 
                animal.StartCoroutine(delayExit());
            }
        }

        IEnumerator delayExit()
        {
            isExiting = true;
            yield return new WaitForSeconds(2);
            if (animal.heardHostileTargets.Count == 0)
            {
                finiteStateMachine.absorbingState = false;
                animal.transform.position = lastPosition;
                finiteStateMachine.GoToDefaultState();    
            }
            else
            {
                isExiting = false;
            }
            
        }

        public void SetTarget(HideoutController hideoutController)
        {
            target = hideoutController;
        }

        public override bool MeetRequirements()
        {
            return target && target.CanHide(animal);
        }
        
        
    }
}