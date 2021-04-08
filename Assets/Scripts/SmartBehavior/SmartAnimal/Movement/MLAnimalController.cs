using System.Collections;
using System.Collections.Generic;
using AnimalsV2;
using AnimalsV2.States;
using AnimalsV2.States.AnimalsV2.States;
using UnityEngine;

public class MLAnimalController : AnimalController
{
    new void Awake()
    {
        base.Awake();
        animalModel = new RabbitModel(new Traits(1f, 100, 100, 
            100, 6.65f, 5f, 
            10,2000, 10, 
            160, 13, 7), 0);
    }
    
    public override void ChangeModifiers(State state)
    {
        //Debug.Log("Changing modifiers for state: " + state.ToString());
        switch (state)
        {
            case GoToFood _:
                //TODO bad practice, hard coded values, this is temporary
                if (animalModel is BearModel || animalModel is WolfModel)
                    HighEnergyState();
                else
                    MediumEnergyState();
                break;
            case GoToWater _:
                MediumEnergyState();
                break;
            case GoToMate _:
                MediumEnergyState();
                break;
            case FleeingState _:
                HighEnergyState();
                break;
            case EatingState _:
                LowEnergyState();
                break;
            case DrinkingState _:
                LowEnergyState();
                break;
            case MatingState _:
                HighEnergyState();
                break;
            case SearchingState _:
                MediumEnergyState();
                break;
            case Idle _:
                LowEnergyState();
                break;
            case Wander _:
                LowEnergyState();
                break;
            case Dead _:
                energyModifier = 0f;
                hydrationModifier = 0f;
                reproductiveUrgeModifier = 0f;
                speedModifier = 0f;
                break;
            default:
                energyModifier = 0.5f;
                hydrationModifier = 0.5f;
                reproductiveUrgeModifier = 20f;
                speedModifier = JoggingSpeed;
                break;
        }
    }

    public override Vector3 getNormalizedScale()
    {
        return new Vector3(1, 1, 1);
    }

    
}
