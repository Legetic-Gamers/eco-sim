using System;
using System.Collections;
using System.Collections.Generic;
using DataCollection;
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

    public event TickDelegate OnTimerUpdate;
    
    public event TickDelegate onDataHandlerUpdate;

    private int count1;
    private int count2;
    private int count3;
    private int tick;

    private IEnumerator TickEventSystem()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(SenseTickEvent());
            tick++;
            if (tick % 2 == 1) OnTimerUpdate?.Invoke();

            if (tick % 4 == 1) 
            {
                StartCoroutine(ParamTickEvent());
                yield return null;
                StartCoroutine(DataHandlerTickEvent());
                yield return null;
            }
            if (tick % 120 == 1)
            {
                StartCoroutine(CollectorTickEvent());
            }
        }
    }

    private IEnumerator ParamTickEvent()
    {
        //while (true)
        //{
        //    count1++;
        //    if (count1 % 30 == 1) Debug.Log("Param" + count1 / 30);
        //    onParamTickEvent?.Invoke();
        //    yield return new WaitForSeconds(2.0f);
    //  }
            //count1++;
            //if (count1 % 30 == 1) Debug.Log("Param" + count1 / 30);
            onParamTickEvent?.Invoke();
            yield break;
    }
    private IEnumerator SenseTickEvent()
    {
       /* while (true)
        {
            count2++;
            if (count2 % 120 == 1) Debug.Log("Sense" + count2 / 120);
            onSenseTickEvent?.Invoke();
            yield return new WaitForSeconds(.5f);
        } */
       //count2++;
       //if (count2 % 120 == 1) Debug.Log("Sense" + count2 / 120);
       onSenseTickEvent?.Invoke();
       yield break;
    }

    private IEnumerator CollectorTickEvent()
    {
        /*while (true)
        {
            
            onCollectorUpdate?.Invoke();
            yield return new WaitForSeconds(60f);
        } */
        onCollectorUpdate?.Invoke();
        yield break;
    }
    
    private IEnumerator DataHandlerTickEvent()
    {
        /*while (true)
        {
            count3++;
            if (count3 % 30 == 1) Debug.Log("DH" + count3 / 30);
            onDataHandlerUpdate?.Invoke();
            yield return new WaitForSeconds(2f);
        } */
        //count3++;
        //if (count3 % 30 == 1) Debug.Log("DH" + count3 / 30);
        onDataHandlerUpdate?.Invoke();
        yield break;
    }


    private void Awake()
    {
        //StartCoroutine("ParamTickEvent");
        //StartCoroutine("SenseTickEvent");
        //StartCoroutine("CollectorTickEvent");
        //StartCoroutine("DataHandlerTickEvent");
        StartCoroutine(TickEventSystem());
    }
}
