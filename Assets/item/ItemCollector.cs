using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    public int collected = 0;
    public int totalRequired = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            collected++;
            Destroy(other.gameObject);
            Debug.Log($"아이템 수집: {collected}/{totalRequired}");
        }
    }

    public bool AllCollected()
    {
        return collected >= totalRequired;
    }
}
