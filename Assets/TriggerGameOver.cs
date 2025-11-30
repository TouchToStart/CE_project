using UnityEngine;
using UnityEngine.AI;

public class TriggerGameOver : MonoBehaviour
{
    public static bool IsGameOverTriggered = false;
    public string playerTag = "Player";

    private bool hasTriggered = false;
    private MonsterActionSound actionSound;
    private MonsterControl monsterControl;

    void Start()
    {
        actionSound = GetComponent<MonsterActionSound>();
        monsterControl = GetComponent<MonsterControl>();

        if (actionSound == null) Debug.LogError("MonsterActionSound 스크립트가 없습니다.");
        if (monsterControl == null) Debug.LogError("MonsterControl 스크립트가 없습니다.");

        // ⭐ 게임 시작 시 플래그 초기화
        IsGameOverTriggered = false;
        hasTriggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 🚨 1단계: 중복 방지 및 플레이어 확인 (CS0414 경고 해결)
        // 이미 잡았거나, 충돌한 대상의 태그가 플레이어가 아니라면 즉시 리턴합니다.
        if (hasTriggered || !other.CompareTag(playerTag))
        {
            return;
        }

        // 🚨 2단계: 무적 로직 (Stunned 상태 확인)
        if (monsterControl != null && monsterControl.currentState == MonsterControl.StunState.Stunned)
        {
            // Debug.Log("몬스터가 무력화 상태여서 잡기 실패.");
            return;
        }

        // --- 1. 게임 오버 처리 시작 (Normal 또는 Slowed 상태에서만 실행) ---
        Debug.Log("플레이어와 접촉했습니다! 게임 오버!");

        // ⭐ 경고 해결 및 중복 방지: 플래그 설정
        hasTriggered = true;
        IsGameOverTriggered = true;

        // --- 2. 몬스터 소리 및 이동 정지 ---
        if (actionSound != null)
        {
            actionSound.PlayCatchSound(); // 잡기 소리 재생
        }

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;
            GetComponent<MazeTracker>().enabled = false;
        }
    }
}