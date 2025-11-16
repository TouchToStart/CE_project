using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MonsterControl : MonoBehaviour
{
    private bool isStunned = false;
    private NavMeshAgent navMeshAgent;
    private MazeTracker mazeTracker; // Creep 에셋에 부착된 스크립트

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        mazeTracker = GetComponent<MazeTracker>(); // MazeTracker 스크립트 가져오기

        if (navMeshAgent == null) Debug.LogError("Nav Mesh Agent 컴포넌트가 없습니다.");
        if (mazeTracker == null) Debug.LogError("MazeTracker 스크립트가 없습니다.");
    }

    // 손전등이 호출하는 무력화 시작 함수
    public void StunStart()
    {
        // 이미 무력화 상태면 아무것도 하지 않음
        if (isStunned) return;

        isStunned = true;
        
        // 1. Maze Tracker 스크립트 비활성화 (추적 로직 정지)
        if (mazeTracker != null)
        {
            mazeTracker.enabled = false;
        }

        // 2. Nav Mesh Agent 이동 정지
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true; // 이동 명령 정지
        }
        
        // 시각적 피드백 (선택 사항)
        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null) renderer.material.color = Color.blue;
    }

    // 손전등이 호출하는 무력화 해제 함수
    public void StunEnd()
    {
        // 무력화 상태가 아니면 아무것도 하지 않음
        if (!isStunned) return;

        isStunned = false;
        
        // 1. Nav Mesh Agent 이동 재개
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = false; // 이동 명령 재개
        }

        // 2. Maze Tracker 스크립트 활성화 (추적 로직 재개)
        if (mazeTracker != null)
        {
            mazeTracker.enabled = true;
        }
        
        // 원래 색상/상태로 되돌립니다.
        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null) renderer.material.color = Color.white;
    }
}