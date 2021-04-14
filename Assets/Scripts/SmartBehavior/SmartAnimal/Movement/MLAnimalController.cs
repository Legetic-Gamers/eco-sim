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
            1,2000, 10, 
            160, 13, 7), 0);
        agent.acceleration *= Time.timeScale;
        agent.angularSpeed *= Time.timeScale;
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
                energyModifier = 0.35f;
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

    protected override void SetPhenotype()
    {
        //gameObject.transform.localScale = getNormalizedScale() * animalModel.traits.size;
    }

    public override void UpdateParameters()
    {
        //The age will increase 2 per 2 seconds.
        animalModel.age += 1;

        // energy
        animalModel.currentEnergy -= (animalModel.age + animalModel.currentSpeed +
                                      animalModel.traits.viewRadius / 10 + animalModel.traits.hearingRadius / 10)
                                     * animalModel.traits.size * energyModifier;

        // hydration
        animalModel.currentHydration -= animalModel.traits.size *
                                        (1 +
                                         animalModel.currentSpeed / animalModel.traits.endurance *
                                         hydrationModifier);

        // reproductive urge
        animalModel.reproductiveUrge += 0.01f * reproductiveUrgeModifier;

    }

    
}
