using System.Collections;
using System.Collections.Generic;
using Menus;
using UnityEngine;
using UnityEngine.EventSystems;

// https://www.youtube.com/watch?v=rnqF6S7PfFA
public class OrbitCameraController : MonoBehaviour
{

    public static OrbitCameraController instance;

    public Transform followTransform;

    public MeshRenderer boundsOfWorld;
    public bool restrictToBounds;

    public bool cameraMovmentEnable;

    [SerializeField]
    private bool navigateWithKeyboard;

    public float normalSpeed;
    public float fastSpeed;
    public float movementSpeed;
    public float movementTime;
    public float rotationAmount;
    public Vector3 zoomAmount;
    public float maxZoom;
    public float minZoom;

    public Vector3 newPosition;
    public Quaternion newRotation;
    public Vector3 newZoom;

    public Vector3 dragStartPosition;
    public Vector3 dragCurrentPosition;
    public Vector3 rotateStartPosition;
    public Vector3 rotateCurrentPosition;


    public Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = camera.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraMovmentEnable && !EventSystem.current.IsPointerOverGameObject())
        {

            if (followTransform != null)
            {
                transform.position = followTransform.position;
            }
            else
            {
                HandleMouseInput();
                HandleMovementInput();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            followTransform = null;
            if (TryGetComponent(out AnimalController animalController))
            {
                animalController.parameterUI.enabled = FindObjectOfType<OptionsMenu>().alwaysShowParameterUI;;
            }
        }
    }

    void HandleMouseInput()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            if ((Input.mouseScrollDelta.y > 0 && newZoom.z < maxZoom)
            || (Input.mouseScrollDelta.y < 0 && newZoom.z > minZoom))
                newZoom += Input.mouseScrollDelta.y * zoomAmount;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }

        if (Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);

                newPosition = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            rotateStartPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(1))
        {
            rotateCurrentPosition = Input.mousePosition;

            Vector3 difference = rotateStartPosition - rotateCurrentPosition;

            rotateStartPosition = rotateCurrentPosition;

            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
        }
    }

    void HandleMovementInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = fastSpeed;
        }
        else
        {
            movementSpeed = normalSpeed;
        }

        if (navigateWithKeyboard)
        {
            float vdir = Input.GetAxis("Vertical");
            float hdir = Input.GetAxis("Horizontal");
            if (vdir != 0)
            {
                newPosition += (transform.forward * movementSpeed * vdir);
            }
            if (hdir != 0)
            {
                newPosition += (transform.right * movementSpeed * hdir);
            }
        }

        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        if (Input.GetKey(KeyCode.R))
        {
            newZoom += zoomAmount;
        }
        if (Input.GetKey(KeyCode.F))
        {
            newZoom -= zoomAmount;
        }

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        if (restrictToBounds)
        {
            if (transform.position.x < -boundsOfWorld.bounds.size.x / 2.0f)
            {
                transform.position = new Vector3(-boundsOfWorld.bounds.size.x / 2.0f, transform.position.y, transform.position.z);
            }
            if (transform.position.x > boundsOfWorld.bounds.size.x / 2.0f)
            {
                transform.position = new Vector3(boundsOfWorld.bounds.size.x / 2.0f, transform.position.y, transform.position.z);
            }
            if (transform.position.z < -boundsOfWorld.bounds.size.z / 2.0f)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, -boundsOfWorld.bounds.size.z / 2.0f);
            }
            if (transform.position.z > boundsOfWorld.bounds.size.z / 2.0f)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, boundsOfWorld.bounds.size.z / 2.0f);
            }
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}
