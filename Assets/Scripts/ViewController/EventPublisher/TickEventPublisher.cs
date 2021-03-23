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
            onParamTickEvent?.Invoke();
            yield return new WaitForSeconds(2.0f / Time.timeScale);
            
        }
    }
    private IEnumerator SenseTickEvent()
    {
        while (true)
        {
            onSenseTickEvent?.Invoke();
            //Debug.Log(Time.timeScale);
            
            //Senses update should scale with timescale.
            yield return new WaitForSeconds(.5f / Time.timeScale);
            
        }
    }

    private IEnumerator CollectorTickEvent()
    {
        while (true)
        {
            onCollectorUpdate?.Invoke();
            yield return new WaitForSeconds(5f);
            
        }
    }


    private void Awake()
    {
        StartCoroutine("ParamTickEvent");
        StartCoroutine("SenseTickEvent");
        StartCoroutine("CollectorTickEvent");
    }
}
