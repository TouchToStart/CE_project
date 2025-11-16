using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MonsterControl : MonoBehaviour
{
    // 몬스터의 현재 상태를 나타내는 열거형
    public enum StunState { Normal, Slowed, Stunned }
    public StunState currentState = StunState.Normal;

    private NavMeshAgent navMeshAgent;
    private MazeTracker mazeTracker;
    private float originalSpeed; // 원래 속도 저장
    
    // 코루틴 참조를 저장하여 중복 실행을 방지
    private Coroutine stunCoroutine; 

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        mazeTracker = GetComponent<MazeTracker>();

        if (navMeshAgent == null) Debug.LogError("Nav Mesh Agent 컴포넌트가 없습니다.");
        if (mazeTracker == null) Debug.LogError("MazeTracker 스크립트가 없습니다.");

        // 원래 속도를 저장합니다.
        if (navMeshAgent != null)
        {
            originalSpeed = navMeshAgent.speed;
        }

        // 시작 시에는 추적을 활성화합니다. (Normal 상태)
        SetMonsterState(StunState.Normal);
    }

    // 외부(FlashlightController)에서 호출하는 무력화 시작 함수
    public void StunStart()
    {
        // 이미 StunCoroutine이 실행 중이라면 중복 시작하지 않습니다.
        if (stunCoroutine != null) return;

        // 새로운 무력화/둔화 코루틴을 시작하고 참조를 저장합니다.
        stunCoroutine = StartCoroutine(HandleStunSequence());
    }

    // 외부(FlashlightController)에서 호출하는 무력화 해제 함수
    public void StunEnd()
    {
        // 무력화/둔화 코루틴이 실행 중이라면 즉시 중지합니다.
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
            stunCoroutine = null;
        }

        // 상태를 Normal로 즉시 되돌립니다.
        SetMonsterState(StunState.Normal);
    }
    
    // 몬스터의 상태를 설정하는 핵심 함수
    private void SetMonsterState(StunState newState)
    {
        if (currentState == newState) return; // 상태 변화가 없으면 리턴

        currentState = newState;
        
        // 상태에 따른 행동 정의
        switch (currentState)
        {
            case StunState.Normal:
                // 1. 이동 재개
                if (navMeshAgent != null)
                {
                    navMeshAgent.isStopped = false;
                    navMeshAgent.speed = originalSpeed; // 원래 속도로 복귀
                }
                // 2. 추적 로직 활성화
                if (mazeTracker != null)
                {
                    mazeTracker.enabled = true;
                }
                GetComponentInChildren<Renderer>().material.color = Color.white;
                break;

            case StunState.Slowed:
                // 1. 속도 둔화 (예: 원래 속도의 50%)
                if (navMeshAgent != null)
                {
                    navMeshAgent.isStopped = false;
                    navMeshAgent.speed = originalSpeed * 0.5f; 
                }
                // 2. 추적 로직 활성화 유지
                if (mazeTracker != null)
                {
                    mazeTracker.enabled = true;
                }
                GetComponentInChildren<Renderer>().material.color = Color.yellow;
                break;
                
            case StunState.Stunned:
                // 1. Nav Mesh Agent 이동 정지
                if (navMeshAgent != null)
                {
                    navMeshAgent.isStopped = true;
                }
                // 2. Maze Tracker 스크립트 비활성화
                if (mazeTracker != null)
                {
                    mazeTracker.enabled = false;
                }
                GetComponentInChildren<Renderer>().material.color = Color.blue;
                break;
        }
    }

    // 둔화와 무력화 단계를 제어하는 코루틴
    private IEnumerator HandleStunSequence()
    {
        // --- 1단계: 둔화 (1초) ---
        SetMonsterState(StunState.Slowed);
        yield return new WaitForSeconds(1.0f);
        
        // --- 2단계: 완전 무력화 ---
        SetMonsterState(StunState.Stunned);
        // 무력화 시간(3초)을 기다립니다.
        yield return new WaitForSeconds(3.0f);
        
        // 4초가 지나면 스스로 Normal 상태로 돌아갑니다.
        // *단, FlashlightController에서 계속 빛을 비추고 있지 않다면 바로 Normal이 됩니다.
        SetMonsterState(StunState.Normal);
        stunCoroutine = null; // 코루틴이 종료되었음을 표시
    }
}