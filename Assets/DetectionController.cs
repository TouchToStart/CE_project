using UnityEngine;

public class DetectionController : MonoBehaviour
{
    private Light spotLight;
    private MonsterControl currentMonster = null;
    private bool wasLightOn = false;

    void Awake()
    {
        spotLight = GetComponent<Light>();
        if (spotLight == null) Debug.LogError("Spot Light에 Light 컴포넌트가 없습니다!");
    }

    void Update()
    {
        if (currentMonster != null)
        {
            // 1. 몬스터가 Stunned 상태라면, 빛 검사를 완전히 무시하고 타이머가 끝날 때까지 기다립니다.
            if (currentMonster.IsFullyStunned())
            {
                return;
            }

            CheckIfHitByLight(currentMonster.transform);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            MonsterControl monster = other.GetComponent<MonsterControl>();
            if (monster != null)
            {
                // 몬스터가 영역에 들어왔을 때 currentMonster에 저장합니다.
                currentMonster = monster;
                // Debug.Log("몬스터 감지 시작: " + other.gameObject.name);
            }
        }
    }

    // 몬스터가 Sphere Collider 영역에서 나갔을 때 실행
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            MonsterControl departingMonster = other.GetComponent<MonsterControl>();

            if (departingMonster != null && currentMonster == departingMonster)
            {
                // 2. 몬스터가 Stunned 상태라면 영역을 벗어나도 StunEnd를 호출하지 않습니다.
                if (!departingMonster.IsFullyStunned())
                {
                    departingMonster.StunEnd();
                }

                Debug.Log("[로그 3] OnTriggerExit: *추적 중이던* 몬스터가 영역을 벗어남! (Stunned 상태가 아니면 해제됨)");
                currentMonster = null;
                wasLightOn = false;
            }
        }
    }

    // DetectionController.cs의 CheckIfHitByLight 함수만 수정합니다.

    void CheckIfHitByLight(Transform monsterTransform)
    {
        Vector3 directionToMonster = monsterTransform.position - transform.position;
        float distanceToMonster = directionToMonster.magnitude;
        float angle = Vector3.Angle(transform.forward, directionToMonster);

        bool withinAngle = angle < spotLight.spotAngle / 2f;
        bool withinRange = distanceToMonster <= spotLight.range;

        // 각도와 거리가 모두 맞을 때 (빛을 비추는 중)
        if (withinAngle && withinRange)
        {
            // 1. StunStart를 계속 호출합니다 (MonsterControl이 중복을 막아줌)
            currentMonster.StunStart();

            // 2. 빛이 켜졌다는 플래그를 설정
            wasLightOn = true;
        }
        else // 빛이 빗나갔거나 범위 밖인 경우
        {
            // 3. ⭐ 핵심 수정: 이전에 빛이 켜져 있었다면 (빛이 막 끊긴 순간) StunEnd를 호출합니다.
            if (wasLightOn)
            {
                currentMonster.StunEnd();
                wasLightOn = false; // 플래그를 꺼서 중복 호출 방지
            }
        }
    }
}