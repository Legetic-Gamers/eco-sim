using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTraits : AnimalTraitModel
{
    private void Start()
    {
        agingDelay = 10.0f; // delay of each age tick in seconds
        ageLimit = 30; // multiplied by agingDelay to get the limit of age in seconds
        StartCoroutine("AgeTimer");
        //hungerDelegateFunctions += HungerDelegate.DecrementHunger;
    }

    private void Update()
    {
        // for testing
        if (isControllable) Move();
    }
}
