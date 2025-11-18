using UnityEngine;

public class SafetyZoneTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerSafetyState safety = other.GetComponent<PlayerSafetyState>();
            if (safety != null)
            {
                safety.isInSafetyZone = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerSafetyState safety = other.GetComponent<PlayerSafetyState>();
            if (safety != null)
            {
                safety.isInSafetyZone = false;
            }
        }
    }
}
