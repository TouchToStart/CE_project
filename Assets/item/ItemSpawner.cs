using UnityEngine;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    public GameObject itemPrefab;
    public Transform[] spawnPoints;
    public int itemCount = 5;

    void Start()
    {
        SpawnItems();
    }

    void SpawnItems()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("스폰 포인트가 없습니다!");
            return;
        }

        // spawnPoints 리스트 복사
        List<Transform> availablePoints = new List<Transform>(spawnPoints);

        // 안전장치: 요청한 개수가 스폰 포인트보다 많으면 자동 조정
        int countToSpawn = Mathf.Min(itemCount, availablePoints.Count);

        for (int i = 0; i < countToSpawn; i++)
        {
            // 항상 남은 범위 내에서 랜덤 선택
            int randIndex = Random.Range(0, availablePoints.Count);
            Transform spawnPoint = availablePoints[randIndex];

            // 아이템 생성
            Instantiate(itemPrefab, spawnPoint.position, Quaternion.identity);
            Debug.Log($"아이템 {i + 1} 생성됨: {spawnPoint.name}");

            // 중복 방지 (사용된 포인트 제거)
            availablePoints.RemoveAt(randIndex);
        }

        Debug.Log($"총 {countToSpawn}개의 아이템이 정상적으로 스폰되었습니다.");
    }
}
