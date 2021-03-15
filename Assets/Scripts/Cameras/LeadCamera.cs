using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeadCamera : MonoBehaviour
{
    Transform target;
    Vector3 offset = new Vector3(0,0,25);
    
    // Start is called before the first frame update
    void Start()
    {
        target = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(-90, target.eulerAngles.y, 0);
    }
}
