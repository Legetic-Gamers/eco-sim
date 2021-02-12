using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungerDelegate : MonoBehaviour
{

    private AnimalTraitModel self;

    public void DecrementHunger() 
    {
        self.hungerLevel--;
    }

    // Start is called before the first frame update
    private void Start()
    {
        self = GetComponent<AnimalTraitModel>();
    }


    // Update is called once per frame
    private void Update()
    {
        DecrementHunger();
    }
}
