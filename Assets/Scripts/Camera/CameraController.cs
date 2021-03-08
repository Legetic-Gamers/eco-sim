using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Move
    public float speed = 25.0f;
    public float height = 2.0f;

    // Rotate
    private float pitch;
    private float yaw;
    public Vector2 pitchMinMax = new Vector2(-40, 60);
    public float sensitivity = 2.0f;
    
    // FollowTarget
    private GameObject target;
    private readonly Vector3 yOffset = new Vector3(0, 2, 0);

    [SerializeField]
    private AnimalSelectPanel animalSelectPanel;
    
    // WASD movement
    private void Move()
    {
        // Positive: D, Negative: A
        var xAxis = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        // Positive: W, Negative: S
        var zAxis = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        transform.Translate(new Vector3(xAxis, 0, zAxis));

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
    private void Rotate()
    {
        // Mouse movement
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        // prevent camera tumble
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }

    // Raycast to get a target at mouse position
    private void GetTarget() 
    {
        // Shoot ray from mouseposition in relative position to the camera
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Variable to store the hit
        RaycastHit hit;
        // If ray != null, out hit (store data in hit)
        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.transform && hit.transform.gameObject.name != "Plane")
            {
                target = hit.collider.gameObject;
                // Print the name of the object
                PrintName(target);
                // Method to handle the hit
                HandleHit(target);
            }
        }
    }
    
    private void HandleHit(GameObject gameObject)
    {
        // See if the gameobject has animalcontroller, if so; we want to show panel with traits
        AnimalController animalController = gameObject.GetComponent<AnimalController>();
        if (animalController && animalController?.animalModel?.traits != null)
        {
            animalSelectPanel.SetTraitText(animalController.animalModel.traits, animalController.gameObject.name);
        }
        else
        {
            animalSelectPanel.Hide();
        }
    }
    
    private void PrintName(GameObject go)
    {
        Debug.Log("You clicked this object: " + go.name);
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        // selct an object to follow it
        if (Input.GetMouseButtonDown(0))
        {
            GetTarget();
        }
        // Esc to deselect target and/or reset zoom/fov level
        if (Input.GetKey(KeyCode.Escape)) 
        {
            target = null;
            
            Camera.main.fieldOfView = 60;
        }
        
        // hold alt to prevent camera rotation
        if (!Input.GetKey(KeyCode.LeftAlt)) Rotate();

        // Follow the target
        if (target) transform.position = target.transform.position - transform.forward * 4 + yOffset;
        else Move();
    }
}
