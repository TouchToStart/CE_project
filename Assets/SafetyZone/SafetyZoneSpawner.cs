using UnityEngine;

public class SafetyZoneSpawner : MonoBehaviour
{
    public GameObject safetyZonePrefab;
    public Transform[] spawnPoints;  // 스폰 위치들
    public int spawnCount = 1;

    void Start()
    {
        SpawnSafetyZones();
    }

    void SpawnSafetyZones()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("스폰 포인트가 없습니다!");
            return;
        }

        // 중복 스폰 방지
        System.Collections.Generic.List<int> usedIndexes = new System.Collections.Generic.List<int>();

        for (int i = 0; i < spawnCount; i++)
        {
            int index = Random.Range(0, spawnPoints.Length);

            // 이미 사용한 포인트라면 다시 뽑기
            while (usedIndexes.Contains(index))
            {
                index = Random.Range(0, spawnPoints.Length);
            }

            usedIndexes.Add(index);

            Transform randomPoint = spawnPoints[index];

            GameObject zone = Instantiate(
                safetyZonePrefab,
                randomPoint.position,
                randomPoint.rotation
            );

            // 스폰된 Zone 이름 보기 좋게 정리
            zone.name = "SafetyZone_" + index;
        }
    }
}
