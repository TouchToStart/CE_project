using UnityEngine;

public class DetectionController : MonoBehaviour
{
    // Light 컴포넌트를 가져오기 위한 변수
    private Light spotLight;
    // '현재 추적 중인' 몬스터 1마리만 저장하기 위한 변수
    private MonsterControl currentMonster = null;

    void Awake()
    {
        // Light 컴포넌트를 가져옵니다.
        spotLight = GetComponent<Light>(); 
        if (spotLight == null) 
        {
            Debug.LogError("Spot Light에 Light 컴포넌트가 없습니다!");
        }
    }

    // --- ★ 수정된 부분 1: Update 함수 추가 ---
    // 몬스터가 감지 영역 안에 머무는 동안(currentMonster가 null이 아닐 때)
    // 매 프레임 빛 검사를 실행합니다.
    void Update()
    {
        // 추적 중인 몬스터가 있을 때만 빛 검사를 수행
        if (currentMonster != null)
        {
            // 매 프레임 빛에 맞았는지 확인하는 핵심 로직 호출
            CheckIfHitByLight(currentMonster.transform);
        }
    }

    // --- ★ 수정된 부분 2: Enter 로직 (추적 '시작') ---
    // 몬스터가 Sphere Collider 영역 안에 들어왔을 때 실행
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            // ★ 중요: 현재 추적 중인 몬스터가 없을 때만 새로 할당합니다.
            // (여러 몬스터가 있어도 1마리만 추적)
            if (currentMonster == null)
            {
                currentMonster = other.GetComponent<MonsterControl>();
                if(currentMonster != null)
                {
                    Debug.Log("[로그 1] OnTriggerEnter: 몬스터 감지 및 추적 시작!");
                }
                else
                {
                    Debug.LogError("OnTriggerEnter: Monster 태그는 맞지만 MonsterControl 스크립트가 없습니다!");
                }
            }
        }
    }

    // --- ★ 수정된 부분 3: Stay 로직 (제거) ---
    // OnTriggerStay는 더 이상 필요 없으므로 함수를 삭제합니다.
    // (모든 로직은 Update()로 이동했습니다)
    /*
    private void OnTriggerStay(Collider other)
    {
        // 이 함수는 이제 사용하지 않습니다.
    }
    */

    // --- ★ 수정된 부분 4: Exit 로직 (추적 '해제') ---
    // 몬스터가 Sphere Collider 영역에서 나갔을 때 실행
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            // 영역을 나간 몬스터의 MonsterControl 컴포넌트를 가져옵니다.
            MonsterControl departingMonster = other.GetComponent<MonsterControl>();

            // ★ 중요: 영역을 나간 몬스터가 우리가 '추적 중이던' 몬스터와 *일치할 때만* 추적을 중지합니다.
            if (departingMonster != null && currentMonster == departingMonster)
            {
                Debug.Log("[로그 3] OnTriggerExit: *추적 중이던* 몬스터가 영역을 벗어남!");
                currentMonster.StunEnd(); // 무력화 해제
                currentMonster = null; // 몬스터 추적 중지
            }
        }
    }

    // 빛에 맞았는지 확인하는 핵심 함수 (이 함수는 수정할 필요 없음)
    void CheckIfHitByLight(Transform monsterTransform)
    {
        Vector3 directionToMonster = monsterTransform.position - transform.position;
        float distanceToMonster = directionToMonster.magnitude;
        float angle = Vector3.Angle(transform.forward, directionToMonster);

        bool withinAngle = angle < spotLight.spotAngle / 2f;
        bool withinRange = distanceToMonster <= spotLight.range;

        // 각도와 거리가 모두 맞을 때만 무력화
        if (withinAngle && withinRange)
        {
            // Debug.Log("[로그 4] 몬스터가 빛의 원뿔 안에 있습니다. (각도, 거리 통과)");
            currentMonster.StunStart(); // 무력화 시작
        }
        else
        {
            // Debug.Log("[로그 5] 몬스터가 빛의 원뿔 밖에 있습니다.");
            currentMonster.StunEnd(); // 무력화 해제
        }
    }
}