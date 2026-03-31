using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    //Internal variables for the coin's behavior and value
    [Header("Score Settings")]
    public int scoreValue = 10;

    [Header("Bounce Settings")]
    public float bounceHeight = 0.25f;
    public float bounceSpeed = 2f;

    // The starting position of the coin, used for calculating the bounce movement
    private Vector3 startPosition;

    // Start is called before the first frame update, we use it to initialise the starting position for the bounce effect
    private void Start()
    {
        // Store the initial position so we bounce relative to it
        startPosition = transform.position;
    }

    // Update is called once per frame, we use it to create a simple bouncing animation for the coin
    private void Update()
    {
        // Simple vertical sine wave movement
        float newY = startPosition.y + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    // OnTriggerEnter is called when another collider enters this object's trigger collider, we use it to detect when the player collects the coin
    private void OnTriggerEnter(Collider other)
    {
        // Only trigger when the player touches the coin
        if (other.CompareTag("Player"))
        {
            // Add score via GameManager
            GameManager.Instance.AddScore(scoreValue);

            // Destroy the coin
            Destroy(gameObject);
        }
    }
}