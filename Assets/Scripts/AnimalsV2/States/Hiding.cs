using System.Collections;
using Menus;
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
            animal.GetComponentInChildren<ParameterUI>(true).SetUIActive(false, true);

        }

        public override void Exit()
        {
            base.Exit();
            target.ExitHideout();
            target = null;
            animal.agent.enabled = true;
            animal.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
            animal.GetComponentInChildren<ParameterUI>(true).SetUIActive(OptionsMenu.alwaysShowParameterUI);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if ((animal.heardHostileTargets.Count == 0 || animal.animalModel.CriticalEnergy || animal.animalModel.CriticalHydration) && !isExiting)
            { 
                animal.StartCoroutine(DelayExit(2));
            }
        }

        IEnumerator DelayExit(float delay)
        {
            isExiting = true;
            
            yield return new WaitForSeconds(delay);
            if ((animal.heardHostileTargets.Count == 0 || animal.animalModel.CriticalEnergy || animal.animalModel.CriticalHydration))
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