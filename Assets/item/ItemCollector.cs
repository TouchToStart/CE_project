// 탈출 조건 + 아이템 UI 설정 2025.11.30
using UnityEngine;
using TMPro;

public class ItemCollector : MonoBehaviour
{
    public int collected = 0;
    public int requiredCount = 6;

    public TextMeshProUGUI itemCountText;  // ✅ UI 연결용

    void Start()
    {
        UpdateUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            collected++;
            UpdateUI();

            Destroy(other.gameObject);
        }
    }

    void UpdateUI()
    {
        if (itemCountText != null)
            itemCountText.text = "item: " + collected + " / " + requiredCount;
    }

    public bool HasEnoughItems()
    {
        return collected >= requiredCount;
    }
}
