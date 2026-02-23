using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    //Declarations
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float rotationSpeed = 15f;
    [Range(0f, 1f)]
    public float airControl = 0.8f;

    [Header("Jump Settings")]
    public float jumpHeight = 3f;
    public float gravity = -20f;
    public float fallMultiplier = 1.5f;
    public float terminalVelocity = -30f;

    [Header("Assisting Features")]
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    [Header("Input References")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;

    // Internal state variables
    private CharacterController controller;
    private Transform cameraTransform;
    private Vector3 velocity;
    private bool isGrounded;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    // Processing 
    void OnEnable()
    {
        // Make sire the actions are enabled so they can read input
        if (moveAction != null) moveAction.action.Enable();
        if (jumpAction != null) jumpAction.action.Enable();
    }

    void OnDisable()
    {
        // Disable the actions when the script is disabled
        if (moveAction != null) moveAction.action.Disable();
        if (jumpAction != null) jumpAction.action.Disable();
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        // Find the main camera in the scene to make movement camera-relative
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        CheckGrounded();
        HandleMovement();
        HandleJumpAndGravity();
    }

    private void CheckGrounded()
    {
        // Check if a sphere at the groundCheck position(Empty) hits anything on the groundMask
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Keep character stuck to the ground slightly when grounded to handle slopes
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        // Manage Coyote Time
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        // Read our input safely from the assigned Action Reference
        Vector2 moveInput = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        float horizontal = moveInput.x;
        float vertical = moveInput.y;

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 moveDir = Vector3.zero;

        if (direction.magnitude >= 0.1f)
        {
            // Calculate movement direction relative to camera
            if (cameraTransform != null)
            {
                // Get camera forward and right vectors, flatten them on the Y axis
                Vector3 camForward = cameraTransform.forward;
                Vector3 camRight = cameraTransform.right;
                camForward.y = 0;
                camRight.y = 0;
                camForward.Normalize();
                camRight.Normalize();

                moveDir = camForward * direction.z + camRight * direction.x;
            }
            else
            {
                // Fallback to world-space movement if no camera is found
                moveDir = direction;
            }

            // Rotate character to face the current movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Apply movement (with slight penalty in the air if airControl is less than 1)
        float currentSpeed = isGrounded ? moveSpeed : moveSpeed * airControl;
        controller.Move(moveDir * currentSpeed * Time.deltaTime);
    }

    private void HandleJumpAndGravity()
    {
        // Manage Jump Buffer Time
        if (jumpAction != null && jumpAction.action.WasPressedThisFrame())
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Execute Jump if buffered input exists and player is within coyote time (Grace period)
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            // Calculate physical jump velocity based on our variable height and gravity
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            
            // Reset the timers so we can't double jump immediately
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }

        // Short Jump Mechanic (variable jump height)
        // If the player lets go of the jump button early while moving up, cut vertical velocity
        if (jumpAction != null && jumpAction.action.WasReleasedThisFrame() && velocity.y > 0f)
        {
            velocity.y *= 0.5f; 
            coyoteTimeCounter = 0f; // Prevent coyote jumping after releasing early
        }

        // Apply the gravity
        float appliedGravity = gravity;
        
        // Use a heavier gravity multiplier when falling for a snappier feel
        if (velocity.y < 0)
        {
            appliedGravity *= fallMultiplier;
        }

        velocity.y += appliedGravity * Time.deltaTime;

        // Clamp to terminal velocity
        if (velocity.y < terminalVelocity)
        {
            velocity.y = terminalVelocity;
        }

        // Check if player bumped their head on a ceiling (don't stick to ceiling)
        if ((controller.collisionFlags & CollisionFlags.Above) != 0)
        {
            if (velocity.y > 0)
            {
                velocity.y = 0;
            }
        }

        // Apply final vertical movement
        controller.Move(velocity * Time.deltaTime);
    }
    
}