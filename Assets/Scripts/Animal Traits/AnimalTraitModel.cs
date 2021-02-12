using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimalTraitModel : MonoBehaviour
{
    protected int age;
    protected int ageLimit;
    protected float agingDelay;

    public bool isControllable = false;


    IEnumerator AgeTimer() 
    {
        while (true) 
        {
            yield return new WaitForSeconds(agingDelay);
            age++;
            if (age > ageLimit) 
            {
                Debug.Log("Animal has passed its age limit.");
                // then have something that kills the animal ( Destroy(object) ) once age reaches a certain number.



                yield break;
            }
            
        }
    }

    [Range(0, 360)]
    public float viewAngle;
    public float viewRadius;
    
    public float hearingRadius;

    private float movementSpeed;

    
    // maybe not delegates, need to work on it to decide
    
    public int hungerLevel;
    private delegate void HungerDelegate();
    private HungerDelegate hungerDelegateFunctions;

    public int thirstLevel;
    public delegate void ThirstDelegate();
    public ThirstDelegate thirstDelegateFunctions;

    public int reproductiveUrge;
    public delegate void ReproductiveUrgeDelegate();
    public ReproductiveUrgeDelegate reproductiveUrgeFunctions;

    public int health;
    public delegate void HealthDelegate();
    public HealthDelegate healthDelegateFunctions;

    /* 
     * if we are to have smell:
     * we should probably have some simple "wind" 
     * that determines the direction of the "smell-cone" 
    */
    [Range(0, 360)]
    private float smellingAngle;
    private float smellingRadius;
    
    // temporary implementation of move for testing purposes
    public void Move()
    {
        // Positive: D, Negative: A
        var xAxis = Input.GetAxis("Horizontal");
        // Positive: W, Negative: S
        var zAxis = Input.GetAxis("Vertical");
        // Mouse turning
        var yAxis = Input.GetAxis("Mouse X");

        transform.Translate(new Vector3(xAxis, 0, zAxis) * 25 * Time.deltaTime);
        transform.eulerAngles += new Vector3(0, yAxis, 0);
    }







}
