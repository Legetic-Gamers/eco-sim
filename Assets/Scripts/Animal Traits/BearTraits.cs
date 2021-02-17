using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTraits : AnimalModel
{
    
    private void Start()
    {
        ageLimit = 30;

        currentEnergy = 10;
        hydration = 10;
        reproductiveUrge = 0;
        // subscribe to the OnTickEvent for parameter handling.
        EventSubscribe();
    }

    private void Update()
    {
        if (isAlive && currentEnergy <= 0 && hydration <= 0)
        {
            isAlive = false; 
            EventUnsubscribe();
            
            // probably doing this in deathState instead
            Destroy(gameObject, 2.0f);
        }
    }
}
