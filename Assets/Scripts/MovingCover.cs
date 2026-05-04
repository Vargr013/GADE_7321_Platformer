using UnityEngine;

public class MovingCover : MonoBehaviour
{
    // how far it moves
    public float moveDistance = 3f;
    // how fast it moves
    public float moveSpeed = 2f;

    // pause at each end
    public float waitTime = 1f;       
    public bool startMovingRight = true;

    private Vector3 startPosition;
    private Vector3 leftTarget;
    private Vector3 rightTarget;

    private Vector3 currentTarget;

    private float waitTimer = 0f;
    private bool isWaiting = false;

    void Start()
    {
        startPosition = transform.position;

        // Calculate left and right targets based on the starting position
        leftTarget = startPosition - Vector3.right * moveDistance;
        rightTarget = startPosition + Vector3.right * moveDistance;

        currentTarget = startMovingRight ? rightTarget : leftTarget;
    }

    void Update()
    {
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                waitTimer = 0f;

                // Switch direction
                currentTarget = (currentTarget == rightTarget) ? leftTarget : rightTarget;
            }

            return;
        }

        // Smooth movement using Lerp
        transform.position = Vector3.Lerp(transform.position, currentTarget, moveSpeed * Time.deltaTime);

        // Check if close enough to target
        if (Vector3.Distance(transform.position, currentTarget) < 0.05f)
        {
            isWaiting = true;
        }
    }
}
