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
        moveDirection = playerControls.ReadValue<Vector2>();
        Vector3 move = transform.right * moveDirection.x + transform.forward * moveDirection.y;
        rb.linearVelocity += move * Time.fixedDeltaTime;
    }

    void Dash(Vector3 direction)
    {
        rb.AddForce(direction.normalized * dashForce, ForceMode.Impulse);   
        
        lastDashTime = Time.time;
    }
}
