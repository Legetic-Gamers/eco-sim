using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickEventPublisher : MonoBehaviour
{
    /// <summary>
    /// Has to be attached to a (preferably empty) gameObject.
    ///
    /// Is a simple ticker that every few seconds publishes an event
    /// telling the subscribed animals to increment or decrement their various parameters (in AnimalModel).
    /// </summary>
    
    public delegate void OnTickDelegate();

    public event OnTickDelegate onTickEvent;

    private IEnumerator TickEvent()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.0f);
            onTickEvent?.Invoke();
        }
    }

    private void Start()
    {
        StartCoroutine("TickEvent");
    }
}
