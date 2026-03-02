using System;
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

    [Header("Settings for Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    private bool isDashOn = false;

    [Header("Pickup States")]
    private bool isShieldOn = false;
    private Coroutine shieldCoroutine;
    private bool isSlowMo = false;
    
    [Header("Animation")]
    private Animator animator;
    private float inputMagnitude;

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
        //Get animator
        animator = GetComponent<Animator>();
        
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
        UpdateAnimations();
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
        // Check for Dash state
        if (isDashOn)
        {
            return; // Skip normal movement while dashing
        }

        // Read our input safely from the assigned Action Reference
        Vector2 moveInput = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        float horizontal = moveInput.x;
        float vertical = moveInput.y;
        
        // Store this for the animator
        inputMagnitude = moveInput.magnitude;

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
        // Check for Dash state
        if (isDashOn)
        {
            return; // Stops gravity from interfering with dash
        }

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
            
            //Trigger animator 
            animator.SetTrigger("Jump");
            
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

    // Blue battery functionality: Slow down time for a short duration
    public void SlowMotion(float duration)
    {
        if (!isSlowMo)
        {
            StartCoroutine(SlowMotionCoroutine(duration));
        }

    }

    private System.Collections.IEnumerator SlowMotionCoroutine(float duration)
    {
        isSlowMo = true;
        
        Time.timeScale = 0.5f; //   Time slowed down to half speed
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Adjust fixed delta time for physics

        yield return new WaitForSecondsRealtime(duration); // Wait for the specified duration in real time

        Time.timeScale = 1f; // Retyrn time back to normal rate
        Time.fixedDeltaTime = 0.02f; // Reset fixed delta time

        isSlowMo = false;
    }

    // Green battery functionality: Activate a temporary shield that protects player from one hit
    public void Shield(float duration)
    {
        if (!isShieldOn)
        {
            if (shieldCoroutine != null)
            {
                StopCoroutine(shieldCoroutine);
            }
            shieldCoroutine = StartCoroutine(ShieldCoroutine(duration));
        }
    }

    private System.Collections.IEnumerator ShieldCoroutine(float duration)
    {
        isShieldOn = true;

        // Here you would typically enable a visual effect or change player state to indicate the shield is active
        yield return new WaitForSeconds(duration); 

        isShieldOn = false;
        // Here you would disable the visual effect or revert player state to indicate the shield has expired

        shieldCoroutine = null; 
    }

    // Red battery functionality: Perform a quick dash in the current movement direction
    public void Dash(float dashForce)
    {
        if (!isDashOn)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private System.Collections.IEnumerator DashCoroutine()
    {
        isDashOn = true;

        Vector3 dashDirection = transform.forward; // Store the dash direction at the moment of activation to ensure consistent movement during the dash

        velocity.y = 0f; // Cancel vertical velocity to prevent unintended vertical movement during the dash and make cleaner dash movement

        float dashTime = 0f;
        while (dashTime < dashDuration)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            dashTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        isDashOn = false;
    }

    // When player gets hit by enemy or hazard, check if shield is active. If it is, consume the shield and prevent damage, else call handover to death-handling
    public void playerHit()
    {
        if (isShieldOn)
        {
            isShieldOn = false; // Consume the shield

            if (shieldCoroutine != null)
            {
                StopCoroutine(shieldCoroutine); // Stop the shield coroutine if it's still running
                shieldCoroutine = null;
            }
            // Here you would also typically disable the visual effect or revert player state to indicate the shield has been consumed
            return; 
        }

        playerDeath(); // If no shield, proceed with normal death handling
    }

    private void playerDeath()
    {
        // Handle player death (e.g., play animation, reset level, etc.)
        Debug.Log("Player has died!");
        
    }
    
    private void UpdateAnimations()
    {
        //Validation
        if (animator == null) return; // Exit if no animator is attached
        
        // Use the magnitude of our intended movement direction
        // 0 if not moving, and 1 if moving
        float animationSpeed = inputMagnitude * moveSpeed;

        // Send the intensity to the animator
        animator.SetFloat("Speed", animationSpeed);
        animator.SetBool("isGrounded", isGrounded);
    }
}