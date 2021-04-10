using System.Collections;
using System.Collections.Generic;
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

    public LayerMask collisionMask;
    
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
    private Quaternion _newRotation;

    // mouse interaction
    private Vector3 _dragStartPosition;
    private Vector3 _dragCurrentPosition;
    private Vector3 _rotateStartPosition;
    private Vector3 _rotateCurrentPosition;
    
    private bool _breakAwayFromLockOn;

    // Start is called before the first frame update
    private void Start()
    {
        instance = this;
        newPosition = transform.position;
        _newRotation = transform.rotation;
        newZoom = camera.transform.localPosition;
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
                    Input.GetAxis("Horizontal") != 0) _breakAwayFromLockOn = true;
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
        
        if (_breakAwayFromLockOn)
        {
            followTransform = null;
            _breakAwayFromLockOn = false;
        }
    }

    private void CheckCollision()
    {
        var cameraTransform = camera.transform;
        Vector3 rayDir = cameraTransform.forward;
        
        // zoom collision
        Ray zoomRay = new Ray(cameraTransform.position, rayDir);
        if (Physics.Raycast(zoomRay, out var hit, collisionMask))
        {
            float hoverError = hit.distance;

            if (hoverError < 5f || newZoom.z > maxZoom)
            {
                newZoom -= new Vector3(0, rayDir.y, rayDir.z) * (zoomAmount.z * 0.2f);
            }
        }
        
        // movement collision
        for (var i = 1; i <= 4; i++)
        {
            rayDir = i switch
            {
                1 => Vector3.back,
                2 => Vector3.forward,
                3 => Vector3.left,
                4 => Vector3.right,
                _ => rayDir
            };
            Ray ray = new Ray(cameraTransform.position, rayDir);
            if (Physics.Raycast(ray, out hit, collisionMask))
            {
                float hoverError = hit.distance;
                
                if (!(hoverError < 5f)) continue;
                
                newPosition -= rayDir * 0.2f;
            }
        }
        // smoothing
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
    }

    private void HandleRotation()
    {
        // mouse rotation (right mouse button)
        if (Input.GetMouseButtonDown(1))
        {
            _rotateStartPosition = Input.mousePosition;
        }
        // apply rotation if button is still pressed
        if (Input.GetMouseButton(1))
        {
            _rotateCurrentPosition = Input.mousePosition;

            Vector3 difference = _rotateStartPosition - _rotateCurrentPosition;

            _rotateStartPosition = _rotateCurrentPosition;

            _newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
        }
        
        // keyboard rotation
        if (Input.GetKey(KeyCode.Q))
        {
            _newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        if (Input.GetKey(KeyCode.E))
        {
            _newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }
        
        // smoothing
        transform.rotation = Quaternion.Lerp(transform.rotation, _newRotation, Time.deltaTime * movementTime);
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
        
        // smoothing
        camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, newZoom, Time.deltaTime * movementTime);
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
                _dragStartPosition = ray.GetPoint(entry);
            }
        }
        // apply movement if button is still pressed
        if (Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out var entry))
            {
                _dragCurrentPosition = ray.GetPoint(entry);

                newPosition = transform.position + _dragStartPosition - _dragCurrentPosition;
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
