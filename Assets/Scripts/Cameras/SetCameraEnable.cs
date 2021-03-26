using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraEnable : MonoBehaviour
{
    public bool enabled;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Camera>().enabled = enabled;
    }
}
