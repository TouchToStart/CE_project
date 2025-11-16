using UnityEngine;

public class PlayerSafetyState : MonoBehaviour
{
    public bool isInSafetyZone = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInSafetyZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SafetyZone"))
        {
            isInSafetyZone = false;
        }
    }
}
