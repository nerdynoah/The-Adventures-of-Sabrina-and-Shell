using BaseCharacter.Items;
using BaseCharacter.Movement;
using UnityEngine;
using static Enums;

public class Movement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform target;
    [SerializeField] Camera cam;
    [SerializeField] Texture2D crosshairImage;
    public Rigidbody body;

    [Header("Top-Down Settings")]
    [SerializeField] float topDownHeight = 10f;
    [SerializeField] float rotationSpeed = 2f;
    [SerializeField] float tiltSpeed = 1f;
    [SerializeField] float minTiltAngle = 30f;
    [SerializeField] float maxTiltAngle = 89f;
    [SerializeField] float zoomSpeed = 5f;
    [SerializeField] float minZoom = 5f;
    [SerializeField] float maxZoom = 30f;
    [SerializeField] float povZoom = 100f;

    private Vector3 offset;
    private readonly RotationAxis axis = RotationAxis.MouseXAndY;
    private CameraMode mode = CameraMode.FirstPerson;

    // First-person settings
    private float SensHor = 2.5f;
    private float SensVert = 2.5f;
    private float MinVert = -90.0f;
    private float MaxVert = 90.0f;
    private float VerticalRot = 0;
    private bool CanMoveMouse = true;
    private bool canMoveCamera = true;
    private KeyCode CameraKey = KeyCode.C;
    private Vector3 ThirdDistance;
    private float rotV;
    private float horizontalRot;
    

    // Top-down rotation variables
    private bool isRotatingTopDown = false;
    private Vector3 lastMousePosition;
    private float currentRotationY = 0f;
    private float currentTiltX = 89f;
    

    void Start()
    {
        mode = GetCameraMode();
        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null) body.freezeRotation = true;

        rotV = transform.eulerAngles.y;
        offset = target.position - transform.position;

        if (mode == CameraMode.TopDownPerspective)
        {
            InitializeTopDownCamera();
        }
        cam.fieldOfView = 90;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void InitializeTopDownCamera()
    {
        currentRotationY = 0f;
        currentTiltX = 89f;
        UpdateTopDownPositionAndRotation();
    }
    public void SetCanLook(bool canLook)
    {
        canMoveCamera = canLook;
    }
    void Update()
    {
        if (canMoveCamera)
        {
            if (mode == CameraMode.FirstPerson)
            {
                HandleFirstPersonRotation();
            }
            else if (mode == CameraMode.TopDownPerspective)
            {
                HandleTopDownRotation();
                HandleTopDownZoom();
            }
            if (Input.GetKey(KeyCode.Mouse2))
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                ZoomScroll(scroll);
            }
        }
    }
    public void ZoomScroll(float amount)
    { 
        cam.fieldOfView += amount * 50f;
        if (cam.fieldOfView < 70f)
        {
            cam.fieldOfView = 70f;
        }
        else if (cam.fieldOfView > 100f)
        {
            cam.fieldOfView = 100f;
        }
    }
    public void SetFOV(float fov, float sens)
    {
        cam.fieldOfView = fov;
        SensHor = sens;
        SensVert = sens;
    }
    void HandleFirstPersonRotation()
    {
        if (!GetCanAxis()) return;

        switch (GetTypeofMouseMovement())
        {
            case 0: BothAxis(); break;
            case 1: XAxis(); break;
            case 2: YAxis(); break;
        }
    }

    void HandleTopDownRotation()
    {
        // Start rotation
        if (Input.GetMouseButtonDown(1))
        {
            isRotatingTopDown = true;
            lastMousePosition = Input.mousePosition;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // End rotation
        if (Input.GetMouseButtonUp(1))
        {
            isRotatingTopDown = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Handle rotation
        if (isRotatingTopDown)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 delta = currentMousePosition - lastMousePosition;

            // Horizontal rotation (around Y axis)
            currentRotationY += delta.x * rotationSpeed * Time.deltaTime;

            // Vertical tilt (X axis rotation)
            currentTiltX -= delta.y * tiltSpeed * Time.deltaTime;
            currentTiltX = Mathf.Clamp(currentTiltX, minTiltAngle, maxTiltAngle);

            UpdateTopDownPositionAndRotation();
            lastMousePosition = currentMousePosition;
        }
    }

    void HandleTopDownZoom()
    {
        
        if (Input.GetKey(KeyCode.Plus))
        {
            topDownHeight = Mathf.Clamp(topDownHeight + zoomSpeed * Time.deltaTime, minZoom, maxZoom);
            UpdateTopDownPositionAndRotation();
        }
        else if (Input.GetKey(KeyCode.Minus))
        {
            topDownHeight = Mathf.Clamp(topDownHeight - zoomSpeed * Time.deltaTime, minZoom, maxZoom);
            UpdateTopDownPositionAndRotation();
        }
    }

    void UpdateTopDownPositionAndRotation()
    {
        if (target == null) return;

        // Position the camera above the target
        // Rotate to look at target with current tilt
        transform.SetPositionAndRotation(target.position + Vector3.up * topDownHeight, Quaternion.Euler(currentTiltX, currentRotationY, 0));

        // Move back to maintain height while tilted
        float distance = topDownHeight / Mathf.Tan(currentTiltX * Mathf.Deg2Rad);
        transform.position -= transform.forward * distance;
    }
    public void SetDistance(float x, float y, float z)
    {
        ThirdDistance = new(x, y, z);
    }
    public Camera GetCamera()
    {
        return cam;
    }

    // Start is called before the first frame update

    /// <summary>
    /// Gets what direction in the Y access your currently looking at.
    /// </summary>
    /// <returns>transform.eulerAngles.x</returns>
    public float LookingYDirection()
    {
        return transform.eulerAngles.x;
    }

    /// <summary>
    /// Mouse X axis movement
    /// </summary>
    public void XAxis()
    {
        transform.Rotate(0, Input.GetAxis("Mouse X") * SensHor, 0);
    }
    /// <summary>
    /// Mouse Y axis movement
    /// </summary>
    public void YAxis()
    {
        VerticalRot -= (Input.GetAxis("Mouse Y") * SensVert);
        VerticalRot = Mathf.Clamp(VerticalRot, MinVert, MaxVert);
        float horizontalRot = transform.localEulerAngles.y;
        transform.localEulerAngles = new Vector3(VerticalRot, horizontalRot, 0);
    }
    /// <summary>
    /// Both X and Y movement
    /// </summary>
    public void BothAxis()
    {
        VerticalRot -= (Input.GetAxis("Mouse Y") * SensVert);
        VerticalRot = Mathf.Clamp(VerticalRot, MinVert, MaxVert);
        float delta = Input.GetAxis("Mouse X") * SensHor;
        float horizontalRot = transform.localEulerAngles.y + delta;
        transform.localEulerAngles = new Vector3(VerticalRot, horizontalRot, 0);

    }
    /// <summary>
    /// See's if the Player can move the mouse. Used during cutscenes
    /// </summary>
    /// <returns>True/False</returns>
    public bool GetCanAxis()
    {
        return CanMoveMouse;
    }
    /// <summary>
    /// Returns how the mouse is moving
    /// </summary>
    /// <returns>0,1,2,3 With 0 being both, 1 being X, 2 being Y, 3 being IDLE</returns>
    public int GetTypeofMouseMovement()
    {
        if (axis == RotationAxis.MouseXAndY)
        {
            return 0;
        }
        if (axis == RotationAxis.MouseY)
        {
            return 2;
        }
        if (axis == RotationAxis.MouseX)
        {
            return 1;
        }
        return 3;
    }
    /// <summary>
    /// gets your Y roatation, 
    /// </summary>
    /// <returns> <code> Vector3 direction = Quaternion.Euler(0, yRotation, 0) * Vector3.forward; </code></returns>
    public Vector3 GetRotation()
    {
        float yRotation = transform.eulerAngles.y;
        Vector3 direction = Quaternion.Euler(0, yRotation, 0) * Vector3.forward;

        return direction;
    }
    public Vector3 GetAnimatedRotation()
    {
        float yRotation = transform.eulerAngles.y;
        Vector3 direction = Quaternion.Euler(0, yRotation - 90, 0) * Vector3.forward;

        return direction;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>Your transform.position</returns>
    public Vector3 GetLocation()
    {
        Vector3 location = transform.position;
        return location;
    }

    // Update is called once per frame
    private void UpdateTopDownPosition()
    {
        // Maintain camera position directly above the Player
        if (target != null)
        {
            transform.position = new Vector3(
                target.position.x,
                target.position.y + topDownHeight,
                target.position.z
            );
        }
    }
    void OnGUI()
    {
        if (crosshairImage != null && mode != CameraMode.TopDownPerspective)
        {
            // Calculate center position
            float x = (Screen.width - crosshairImage.width) / 2;
            float y = (Screen.height - crosshairImage.height) / 2;

            // Draw the crosshair
            GUI.DrawTexture(new Rect(x, y, crosshairImage.width, crosshairImage.height), crosshairImage);
        }
    }
    void LateUpdate()
    {
        if (mode == CameraMode.ThirdPerson)
        {
            /*
            VerticalRot -= (Input.GetAxis("Mouse Y") * SensVert);
            VerticalRot = Mathf.Clamp(VerticalRot, MinVert, MaxVert);

            
            rotV += Input.GetAxis("Mouse X") * SensHor;

            Quaternion rotation = Quaternion.Euler(0, rotV, 0);
            transform.position = target.position - (rotation * offset);
            transform.rotation = new(VerticalRot, transform.rotation.y, transform.rotation.z, transform.rotation.w);
            
            transform.LookAt(target);

            //transform.localEulerAngles = new Vector3(VerticalRot, horizontalRot, 0);
            */
            rotV += Input.GetAxis("Mouse X") * SensHor;
            Quaternion rotation = Quaternion.Euler(0, rotV, 0);
            transform.position = target.position - (rotation * offset);

            transform.LookAt(target);
            YAxis();

        }

    }
    /// <summary>
    /// Get the raypoint of where your looking.
    /// </summary>
    /// <param name="rockTMP">The weapon to be used during the process to cause innacuracsyes</param>
    /// <param name="aim">Player aim</param>
    /// <returns>Raycast hits</returns>
    public RaycastHit GetRayPoint(Weapon rockTMP, float aim)
    {
        Ray ray;
        Ray ogRay;
        Vector3 ogpoint = new Vector3(((GetCamera().pixelWidth) / 2), ((GetCamera().pixelHeight) / 2), 0.5f);
        //Debug.Log($"Aim: {rockTMP.GetSphereAccuracy(true, aim)}");
        Vector3 point = new Vector3(ogpoint.x + rockTMP.GetSphereAccuracy(true, aim), ogpoint.y + rockTMP.GetSphereAccuracy(true, aim), ogpoint.z + rockTMP.GetSphereAccuracy(true, aim));
        Vector3 mousePosition = Input.mousePosition;
        Vector3 ThemousePosition = new Vector3(mousePosition.x + rockTMP.GetSphereAccuracy(true, aim), mousePosition.y + rockTMP.GetSphereAccuracy(true, aim), mousePosition.z + rockTMP.GetSphereAccuracy(true, aim));
        if (GetCameraMode() == CameraMode.FirstPerson || GetCameraMode() == CameraMode.ThirdPerson)
        {
            ray = GetCamera().ScreenPointToRay(point);
            ogRay = GetCamera().ScreenPointToRay(ogpoint);
        }
        else // TopDownPerspective
        {
            ray = GetCamera().ScreenPointToRay(ThemousePosition);
            ogRay = GetCamera().ScreenPointToRay(ThemousePosition);
        }
        int includeMask = (1 << 0) | (1 << 10);
        Physics.Raycast(ray, out RaycastHit hit, 1000, includeMask);
        return hit;
    }
    public RaycastHit GetRayPoint(float rngAim, float aim)
    {
        Ray ray;
        Ray ogRay;
        Vector3 ogpoint = new Vector3(((GetCamera().pixelWidth) / 2), ((GetCamera().pixelHeight) / 2), 0.5f);
        Vector3 point = new Vector3(ogpoint.x + Mathf.Max((rngAim + aim),0) * (Random.value - 0.5f), ogpoint.y + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f), ogpoint.z + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f));
        Vector3 mousePosition = Input.mousePosition;
        Vector3 ThemousePosition = new Vector3(mousePosition.x + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f), mousePosition.y + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f), mousePosition.z + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f));
        if (GetCameraMode() == CameraMode.FirstPerson || GetCameraMode() == CameraMode.ThirdPerson)
        {
            ray = GetCamera().ScreenPointToRay(point);
            ogRay = GetCamera().ScreenPointToRay(ogpoint);
        }
        else // TopDownPerspective
        {
            ray = GetCamera().ScreenPointToRay(ThemousePosition);
            ogRay = GetCamera().ScreenPointToRay(ThemousePosition);
        }
        Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 10);
        return hit;
    }
    public RaycastHit GetRayPoint(float rngAim, float aim, int layers)
    {
        Ray ray;
        Ray ogRay;
        Vector3 ogpoint = new Vector3(((GetCamera().pixelWidth) / 2), ((GetCamera().pixelHeight) / 2), 0.5f);
        Vector3 point = new Vector3(ogpoint.x + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f), ogpoint.y + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f), ogpoint.z + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f));
        Vector3 mousePosition = Input.mousePosition;
        Vector3 ThemousePosition = new Vector3(mousePosition.x + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f), mousePosition.y + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f), mousePosition.z + Mathf.Max((rngAim + aim), 0) * (Random.value - 0.5f));
        if (GetCameraMode() == CameraMode.FirstPerson || GetCameraMode() == CameraMode.ThirdPerson)
        {
            ray = GetCamera().ScreenPointToRay(point);
            ogRay = GetCamera().ScreenPointToRay(ogpoint);
        }
        else // TopDownPerspective
        {
            ray = GetCamera().ScreenPointToRay(ThemousePosition);
            ogRay = GetCamera().ScreenPointToRay(ThemousePosition);
        }
        Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layers);
        return hit;
    }
}
