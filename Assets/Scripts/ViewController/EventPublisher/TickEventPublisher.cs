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
    
    public event TickDelegate onDataHandlerUpdate;

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
            yield return new WaitForSeconds(.5f / Time.timeScale);
        }
    }

    private IEnumerator CollectorTickEvent()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f / Time.timeScale);
            onCollectorUpdate?.Invoke();
        }
    }
    
    private IEnumerator DataHandlerTickEvent()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f / Time.timeScale);
            onDataHandlerUpdate?.Invoke();
        }
    }


    private void Awake()
    {
        StartCoroutine("ParamTickEvent");
        StartCoroutine("SenseTickEvent");
        StartCoroutine("CollectorTickEvent");
        StartCoroutine("DataHandlerTickEvent");
    }
}
