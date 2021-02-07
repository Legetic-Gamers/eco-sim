using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Move
    public float speed = 25.0f;
    public float height = 50.0f;

    // Rotate
    public Vector2 pitchMinMax = new Vector2(-20, 40);
    private float pitch;
    private float yaw;
    private float sensitivity = 2.0f;
    
    // FollowTarget
    [Tooltip("Must be a valid tag (including correct capitalization) for targeting to work!")]
    public string target_tag = "Animal";
    private GameObject Target;
    private Vector3 offset = new Vector3(0, 2, 0);
    
    // WASD movement
    void Move()
    {
        // Positive: D, Negative: A
        float X_Axis = Input.GetAxis("Horizontal");
        // Positive: W, Negative: S
        float Z_Axis = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(X_Axis, 0, Z_Axis) * speed * Time.deltaTime);

        // This is set so that the camera always stays at a certain height
        transform.position = new Vector3(transform.position.x, height, transform.position.z);

        // Change the height by scrolling while holding LeftAlt key.
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            height -= Input.GetAxis("Mouse ScrollWheel") * speed;
            height = Mathf.Max(height, 0);
        }
        // Else scrolling will set the zoom level (FoV).
        else 
        {
            Camera.main.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * speed;
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 15, 100);
        }
    }

    // Camera rotation
    void Rotate()
    {
        // Mouse movement
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        // prevent camera tumble
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }

    // Raycast to get a target at mouse position
    void GetTarget() 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == target_tag) 
            {
                Target = hit.collider.gameObject;
                Debug.Log("Raycast hit " + Target.name + "!");
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Selects an object with a certain tag at mouseover after pressing T so it can be followed
        if (Input.GetMouseButtonDown(0))
        {
            GetTarget();
        }
        // Esc to deselect target and/or reset zoom/fov level
        if (Input.GetKey(KeyCode.Escape)) 
        {
            Target = null;
            Camera.main.fieldOfView = 60;
        }
        
        // hold alt to prevent camera rotation
        if (!Input.GetKey(KeyCode.LeftAlt)) Rotate();

        // Follow the target
        if (Target != null) transform.position = Target.transform.position - transform.forward * 4 + offset;
        else Move();
    }
}
