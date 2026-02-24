using UnityEngine;

public class Pickups : MonoBehaviour
{
    public enum typeBattery
    {
        RedBattery,
        BlueBattery,
        GreenBattery
    }

    [Header ("Battery Settings")]
    public typeBattery batteryType;
    public float dashForce = 12f;
    public float Duration = 5f;

    // Check which battery player collided with and calls subsequent procedure. 
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            switch (batteryType)
            {
                case typeBattery.RedBattery:
                    player.Dash(dashForce); break;

                case typeBattery.BlueBattery:
                    player.SlowMotion(Duration); break;

                case typeBattery.GreenBattery:
                    player.Shield(Duration); break;
            }
            Destroy(gameObject);
        }

    }

    
}
