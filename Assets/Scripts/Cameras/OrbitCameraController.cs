using System.Collections;
using System.Collections.Generic;
using Menus;
using UnityEngine;
using UnityEngine.EventSystems;

// https://www.youtube.com/watch?v=rnqF6S7PfFA
public class OrbitCameraController : MonoBehaviour
{

    public static OrbitCameraController instance;
    public new Camera camera;

    public Transform followTransform;

    public MeshRenderer boundsOfWorld;
    public bool restrictToBounds;

    // collision detection
    public LayerMask collisionMask;
    private const float HitThreshold = 1.5f;
    
    public bool cameraMovementEnable;
    public bool navigateWithKeyboard;

    // parameters
    public float normalSpeed;
    public float fastSpeed;
    public float movementSpeed;
    public float movementTime;
    public float rotationAmount;
    public Vector3 zoomAmount;
    public float maxZoom;
    public float minZoom;

    // camera transform
    public Vector3 newPosition;
    public Vector3 newZoom;
    private Quaternion newRotation;

    // mouse interaction
    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;
    private Vector3 rotateStartPosition;
    private Vector3 rotateCurrentPosition;
    
    private bool breakAwayFromLockOn;
    private bool showUI;
    // Start is called before the first frame update
    private void Start()
    {
        instance = this;
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = camera.transform.localPosition;
        showUI = OptionsMenu.alwaysShowParameterUI;
    }

    // Update is called once per frame
    private void Update()
    {
        if (cameraMovementEnable && !EventSystem.current.IsPointerOverGameObject())
        {
            if (followTransform)
            {
                newPosition = followTransform.position;

                // breakaway input
                if (Input.GetKeyDown(KeyCode.Escape) || 
                    Input.GetAxis("Vertical") != 0 || 
                    Input.GetAxis("Horizontal") != 0) breakAwayFromLockOn = true;
            }
            else
            {
                HandleMouseMovement();
                HandleKeyboardMovement();
            }
        }

        HandleRotation();
        HandleZoom();
        CheckCollision();
        
        if (breakAwayFromLockOn)
        {
            if (followTransform && followTransform.gameObject.TryGetComponent(out AnimalController animalController))
            {
                animalController.parameterUI.gameObject.SetActive(showUI);
            }
            followTransform = null;
            breakAwayFromLockOn = false;
        }
    }

    private void CheckCollision()
    {
        var cameraTransform = camera.transform;
        var cameraWorldPos = cameraTransform.position;
        var cameraLocalPos = cameraTransform.localPosition;
        var cameraRigPos = transform.position;
        
        RaycastHit hit;
        
        var diffVector = newZoom - cameraLocalPos;
        var zoomDiff = diffVector.magnitude;
        // zoom collision
        if(zoomDiff > 0)
        {
            // 1 if same dir, 0 if perpendicular, -1 if opposite dir
            var dotProd = Vector3.Dot(diffVector.normalized, cameraTransform.forward);

            // dont check collisions if zooming out
            if (dotProd > 0)
            {
                var zoomRay = new Ray(cameraWorldPos, diffVector);
                if (Physics.Raycast(zoomRay, out hit, 15, collisionMask))
                {
                    var hitError = hit.distance - zoomDiff;

                    if (hitError < HitThreshold)
                    {
                        diffVector = (hit.point - cameraWorldPos).normalized * (hit.distance - HitThreshold);
                        // don't zoom past the threshold
                        newZoom = cameraLocalPos + diffVector;
                    }
                }
            }
            // smoothing / set new zoom
            cameraTransform.localPosition = Vector3.Lerp(cameraLocalPos, newZoom, Time.deltaTime * movementTime);
        }

        
        
        /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */

        // set to newRotation, but keep the original
        var tempRotation = transform.rotation;
        transform.rotation = newRotation;
        
        // get new position of camera after rotating the rig
        var cameraRotated = cameraTransform.position;
        diffVector = cameraRotated - cameraWorldPos;
        
        // restore original rotation
        transform.rotation = tempRotation;
        var rotationDiff = diffVector.magnitude;
        // rotation collision
        if(rotationDiff > 0)
        {
            var zoomRay = new Ray(cameraWorldPos, diffVector);
            if (Physics.Raycast(zoomRay, out hit, 15, collisionMask))
            {
                var hitError = hit.distance - rotationDiff;

                if (hitError < HitThreshold)
                {
                    // don't rotate (because I won't calculate where to put the rotation within the threshold)
                    newRotation = transform.rotation;
                }
            }
            // smoothing / set new rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        }
        
        
        
        /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */



        diffVector = newPosition - cameraRigPos;
        var movementDiff = diffVector.magnitude;
        // movement collision
        if (movementDiff > 0)
        {
            var moveRay = new Ray(cameraWorldPos + cameraTransform.forward * 0.5f, diffVector);
            if (Physics.Raycast(moveRay, out hit, 15, collisionMask))
            {
                var hitError = hit.distance - movementDiff;

                if (hitError < HitThreshold)
                {
                    diffVector = (hit.point - cameraWorldPos).normalized * (hit.distance - HitThreshold);
                    // don't move past the threshold
                    newPosition = cameraRigPos + diffVector;
                }
            }
            // don't move on the y-axis
            newPosition = new Vector3(newPosition.x, 0, newPosition.z);
            // smoothing / set new position
            transform.position = Vector3.Lerp(cameraRigPos, newPosition, Time.deltaTime * movementTime);
        }
    }

    private void HandleRotation()
    {
        // mouse rotation (right mouse button)
        if (Input.GetMouseButtonDown(1))
        {
            rotateStartPosition = Input.mousePosition;
        }
        // apply rotation if button is still pressed
        if (Input.GetMouseButton(1))
        {
            rotateCurrentPosition = Input.mousePosition;

            Vector3 difference = rotateStartPosition - rotateCurrentPosition;

            rotateStartPosition = rotateCurrentPosition;

            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
        }
        
        // keyboard rotation
        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }
    }

    private void HandleZoom()
    {
        // zoom strength based on distance
        if (newZoom.z > -15) zoomAmount = new Vector3(0, -0.5f, 0.5f);
        else if (newZoom.z < -14 && newZoom.z > -50) zoomAmount = new Vector3(0, -2, 2);
        else if (newZoom.z < -40 && newZoom.z > -150) zoomAmount = new Vector3(0, -5, 5);
        else if (newZoom.z > minZoom) zoomAmount = new Vector3(0, -10, 10);
        
        // scroll zoom
        if (Input.mouseScrollDelta.y != 0)
        {
            if ((Input.mouseScrollDelta.y > 0 && newZoom.z < maxZoom)
                || (Input.mouseScrollDelta.y < 0 && newZoom.z > minZoom))
                newZoom += Input.mouseScrollDelta.y * zoomAmount;
        }
        
        // key zoom
        if (Input.GetKey(KeyCode.R))
        {
            newZoom += zoomAmount * 0.5f;
        }
        if (Input.GetKey(KeyCode.F))
        {
            newZoom -= zoomAmount * 0.5f;
        }
    }

    private void HandleMouseMovement()
    {
        // left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

            if (plane.Raycast(ray, out var entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }
        // apply movement if button is still pressed
        if (Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out var entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);

                newPosition = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }
    }

    private void HandleKeyboardMovement()
    {
        // "sprinting"
        movementSpeed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : normalSpeed;

        // WASD/arrows input
        if (navigateWithKeyboard)
        {
            float vDir = Input.GetAxis("Vertical");
            float hDir = Input.GetAxis("Horizontal");
            
            if (vDir != 0) 
                newPosition += transform.forward * (movementSpeed * vDir);
            if (hDir != 0) 
                newPosition += transform.right * (movementSpeed * hDir);
        }
        
        if (!restrictToBounds) return;
        
        if (transform.position.x < -boundsOfWorld.bounds.size.x / 2.0f)
            transform.position = new Vector3(-boundsOfWorld.bounds.size.x / 2.0f, transform.position.y, transform.position.z);
            
        if (transform.position.x > boundsOfWorld.bounds.size.x / 2.0f)
            transform.position = new Vector3(boundsOfWorld.bounds.size.x / 2.0f, transform.position.y, transform.position.z);
            
        if (transform.position.z < -boundsOfWorld.bounds.size.z / 2.0f)
            transform.position = new Vector3(transform.position.x, transform.position.y, -boundsOfWorld.bounds.size.z / 2.0f);
            
        if (transform.position.z > boundsOfWorld.bounds.size.z / 2.0f)
            transform.position = new Vector3(transform.position.x, transform.position.y, boundsOfWorld.bounds.size.z / 2.0f);
    }
}
