using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using ViewController;

namespace AnimalsV2.States
{
    public class Hiding : State
    {

        private HideoutController target;
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
            base.Enter();
            target.EnterHideout();
            finiteStateMachine.isLocked = true;
            isExiting = false;
            animal.agent.enabled = false;
            animal.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            Canvas canvas = animal.GetComponentInChildren<Canvas>();
            if (canvas)
            {
                canvas.gameObject.SetActive(false);
            }

        }

        public override void Exit()
        {
            base.Exit();
            target.ExitHideout();
            target = null;
            animal.agent.enabled = true;
            animal.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
            ParameterUI parameterUI = animal.GetComponentInChildren<ParameterUI>();
            if (parameterUI)
            {
                parameterUI.UpdateUI();
            }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (animal.heardHostileTargets.Count == 0 && !isExiting)
            { 
                animal.StartCoroutine(DelayExit());
            }
        }

        IEnumerator DelayExit()
        {
            isExiting = true;
            yield return new WaitForSeconds(2);
            if (animal.heardHostileTargets.Count == 0)
            {
                finiteStateMachine.isLocked = false;
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