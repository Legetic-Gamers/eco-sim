using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public Vector3 rotationAmount;

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.Rotate(rotationAmount);
    }
}
