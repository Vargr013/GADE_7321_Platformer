using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SecretPathController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The ActivatablePlatform that extends when the hero enters the trigger.")]
    public ActivatablePlatform secretPlatform;

    [Header("Settings")]
    [Tooltip("If true, this trigger disables itself after firing once.")]
    public bool oneShot = true;

    private bool hasActivated;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasActivated && oneShot) return;
        if (secretPlatform == null) return;
        if (!other.CompareTag("Player")) return;
        if (other.GetComponent<PlayerController>() == null) return;

        secretPlatform.Activate();
        hasActivated = true;

        if (oneShot) gameObject.SetActive(false);
    }
}
