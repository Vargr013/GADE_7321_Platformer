using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    public Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);
    public float distance = 5f;

    [Header("Camera Control Settings")]
    public float sensitivity = 0.2f;
    public float smoothTime = 0.12f;
    public Vector2 pitchMinMax = new Vector2(-40f, 85f);

    [Header("Input References")]
    public InputActionReference lookAction;

    // Internal state variables
    private Vector3 currentRotation;
    private Vector3 smoothVelocity = Vector3.zero;
    private float yaw;   // Left/Right rotation
    private float pitch; // Up/Down rotation

    void OnEnable()
    {
        //Make sure the look action is enabled 
        if (lookAction != null) lookAction.action.Enable();

        // Lock the cursor to the center of the screen and hide it for standard PC controls
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDisable()
    {
        //Validation Check
        if (lookAction != null) lookAction.action.Disable();

        // Free the cursor when the script/game stops
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // We use LateUpdate for the camera to ensure the player has completely finished moving this frame
    // before the camera calculates where it needs to be to follow them.
    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("ThirdPersonCamera needs a target assigned in the inspector!");
            return;
        }

        // 1. Read lookInput
        Vector2 lookInput = lookAction != null ? lookAction.action.ReadValue<Vector2>() : Vector2.zero;

        // 2. Calculate Rotation Angles
        yaw += lookInput.x * sensitivity;
        pitch -= lookInput.y * sensitivity; // Subtracting so pushing up looks up
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        // 3. Smooth the Rotation
        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref smoothVelocity, smoothTime);

        // 4. Apply Rotation and Position
        transform.eulerAngles = currentRotation;

        Vector3 targetFocusPosition = target.position + targetOffset;
        
        // Move the camera backward by distance along its new forward axis
        transform.position = targetFocusPosition - transform.forward * distance;
    }
}
