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
    /// telling the subscribed animals to increment or decrement their various parameters (in AnimalModel),
    /// and telling the senses to scan for targets
    /// </summary>
    
    public delegate void TickDelegate();

    public event TickDelegate onParamTickEvent;
    public event TickDelegate onSenseTickEvent;
    public event TickDelegate onCollectorUpdate;

    private IEnumerator ParamTickEvent()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.0f);
            onParamTickEvent?.Invoke();
        }
    }
    private IEnumerator SenseTickEvent()
    {
        while (true)
        {
            yield return new WaitForSeconds(.5f);
            onSenseTickEvent?.Invoke();
        }
    }

    private IEnumerator CollectorTickEvent()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            onCollectorUpdate?.Invoke();
        }
    }


    private void Awake()
    {
        StartCoroutine("ParamTickEvent");
        StartCoroutine("SenseTickEvent");
        StartCoroutine("CollectorTickEvent");
    }
}
