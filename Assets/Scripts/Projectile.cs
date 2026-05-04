using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 8f;
    public float lifetime = 6f;

    private Vector3 moveDirection;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector3 direction)
    {
        // Ensure the direction is normalized to maintain consistent speed
        moveDirection = direction.normalized;
    }

    void Update()
    {
        // Move the projectile in the set direction
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit!");
        }

        // Prevent destroying immediately if hitting the turret itself
        if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}