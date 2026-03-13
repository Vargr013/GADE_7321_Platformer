using UnityEngine;

public class PlatformSpin : MonoBehaviour
{
    // The speed at which the platform spins
    public Transform pivotPoint;
    public float SpinSpeed = 100f;
    //private Transform player;

    //private float liftOffset = 0.05f; // tiny lift to prevent sinking



    // Update is called once per frame
    void FixedUpdate()
    {
        transform.RotateAround(pivotPoint.position, Vector3.up, SpinSpeed * Time.deltaTime);

        /*if (player != null)
        {
            player.RotateAround(pivotPoint.position, Vector3.up, SpinSpeed * Time.deltaTime);
        }*/
        // Rotate player with a small vertical offset
    }


    /*private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Player"))
        {
            player = hit.transform;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = null;
        }
    }*/
}
