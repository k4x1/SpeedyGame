using UnityEditor.Build;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float dashForce = 10f; 
    public float dashCooldown = 2f;
    public float mouseSensitivity = 100f;
    public Transform playerCamera;

    private Rigidbody rb;
    private float lastDashTime;
    private float xRotation = 0f;
    
    public InputAction playerControls;
    public Vector2 moveDirection = Vector2.zero;
    public float moveSpeed = 1f;
    public float decayRate = 1f;

    public Camera cameraRef;

    public Vector3 acceleration;

    private void OnEnable()
    {
        playerControls.Enable();
    }   
    private void OnDisable()
    {
        playerControls.Disable();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastDashTime = -dashCooldown; // Initialize to allow immediate dash
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
    }

    void LateUpdate()
    {
        HandleCameraRotation();

    }
    private void FixedUpdate()
    {
        HandleMove();
    }
    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamp vertical rotation

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMove()
    {
        Vector3 cameraForward = playerCamera.transform.forward;
        cameraForward.Normalize();
        acceleration = cameraForward * moveSpeed;
        float cosineSim = CosineSimilarity(cameraForward, rb.linearVelocity);
        float exponentialFactor = Mathf.Exp(-cosineSim* decayRate);

        rb.linearVelocity += (acceleration * exponentialFactor) * Time.fixedDeltaTime;
        Debug.Log(acceleration * exponentialFactor);
    }



    public static float CosineSimilarity(Vector3 a, Vector3 b)
    {
        float dotProduct = Vector3.Dot(a, b);
        float magnitudeA = a.magnitude;
        float magnitudeB = b.magnitude;
        return (dotProduct / (magnitudeA * magnitudeB)+1)/2;
    }

    void Dash(Vector3 direction)
    {
        rb.AddForce(direction.normalized * dashForce, ForceMode.Impulse);

        lastDashTime = Time.time;
    }
}
