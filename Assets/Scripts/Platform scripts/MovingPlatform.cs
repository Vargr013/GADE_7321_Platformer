using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public enum MovementDirection
    {
        Horizontal,
        Vertical
    }

    [Header("Movement Settings")]
    // Choose whether the platform moves side to side or along the level depth
    public MovementDirection movementDirection = MovementDirection.Horizontal;
    // Distance controls how far the platform moves from its start point
    public float distance = 5f;
    // Speed controls how quickly the platform moves back and forth
    public float speed = 2f;
    // Local space follows the platform's own right/up direction
    public bool useLocalSpace = true;

    [Header("Start Offset")]
    // Random start offset stops all platforms from moving in sync
    public bool randomizeStartOffset = true;
    // Range of time added to this platform's movement cycle at start
    public Vector2 randomStartOffsetRange = new Vector2(0f, 2f);

    [Header("Rider Settings")]
    // Moves the player with the platform while they are standing on it
    public bool moveRiders = true;

    private readonly HashSet<Transform> riders = new HashSet<Transform>();
    // Used to calculate the movement offset every physics frame
    private Vector3 startPosition;
    private Vector3 previousPosition;
    private float startOffset;

    private void Awake()
    {
        startPosition = transform.position;
        previousPosition = transform.position;

        // Pick a random cycle offset when the platform starts
        if (randomizeStartOffset)
        {
            startOffset = Random.Range(randomStartOffsetRange.x, randomStartOffsetRange.y);
        }
    }

    private void FixedUpdate()
    {
        // Pick the movement direction from the Inspector setting
        Vector3 axis = GetMovementAxis();

        // PingPong moves from 0 to distance, then back to 0
        float movementTime = Time.time + startOffset;
        Vector3 offset = axis.normalized * Mathf.PingPong(movementTime * speed, distance);

        Vector3 targetPosition = startPosition + offset;
        // Delta is how far the platform moved this frame
        Vector3 delta = targetPosition - previousPosition;

        transform.position = targetPosition;

        if (moveRiders && delta.sqrMagnitude > 0f)
        {
            MoveRiders(delta);
        }

        previousPosition = targetPosition;
    }

    private Vector3 GetMovementAxis()
    {
        if (movementDirection == MovementDirection.Vertical)
        {
            return useLocalSpace ? transform.forward : Vector3.forward;
        }

        return useLocalSpace ? transform.right : Vector3.right;
    }

    private void MoveRiders(Vector3 delta)
    {
        foreach (Transform rider in riders)
        {
            if (rider == null) continue;

            // CharacterController players must be moved with Move, because they are affected by gravity and collisions
            CharacterController controller = rider.GetComponent<CharacterController>();
            if (controller != null && controller.enabled)
            {
                controller.Move(delta);
            }
            else
            {
                rider.position += delta;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Add the player as a rider when they touch the platform trigger
        if (other.CompareTag("Player"))
        {
            riders.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Stop carrying the player once they leave the platform trigger
        if (other.CompareTag("Player"))
        {
            riders.Remove(other.transform);
        }
    }
}
