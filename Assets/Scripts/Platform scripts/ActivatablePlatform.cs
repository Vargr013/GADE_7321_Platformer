using System.Collections.Generic;
using UnityEngine;

public class ActivatablePlatform : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("Offset (local or world) the platform moves to once activated. The platform starts at its current position on Awake and lerps toward this offset.")]
    public Vector3 targetPosition = new Vector3(0f, 3f, 0f);

    [Header("Movement Settings")]
    [Tooltip("How fast the platform moves toward the target position (units per second).")]
    public float moveSpeed = 3f;

    [Tooltip("If true, targetPosition is treated as a local-space offset from the start position. If false, it is treated as a world-space point.")]
    public bool useLocalSpace = true;

    [Header("Rider Settings")]
    [Tooltip("Moves the player with the platform while they are standing on it.")]
    public bool carryRiders = true;

    private readonly HashSet<Transform> riders = new HashSet<Transform>();

    private Vector3 startPosition;
    private Vector3 previousPosition;
    private Vector3 worldTarget;
    private bool isActivated;

    private void Awake()
    {
        startPosition = transform.position;
        previousPosition = transform.position;
        worldTarget = useLocalSpace ? startPosition + targetPosition : targetPosition;
    }

    private void FixedUpdate()
    {
        if (!isActivated)
        {
            return;
        }

        Vector3 target = worldTarget;
        Vector3 nextPosition = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.fixedDeltaTime);
        Vector3 delta = nextPosition - previousPosition;

        transform.position = nextPosition;
        previousPosition = nextPosition;

        if (carryRiders && delta.sqrMagnitude > 0f)
        {
            MoveRiders(delta);
        }

        if (nextPosition == target)
        {
            isActivated = false;
        }
    }

    public void Activate()
    {
        isActivated = true;
    }

    private void MoveRiders(Vector3 delta)
    {
        foreach (Transform rider in riders)
        {
            if (rider == null) continue;

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
        if (other.CompareTag("Player"))
        {
            riders.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            riders.Remove(other.transform);
        }
    }
}
