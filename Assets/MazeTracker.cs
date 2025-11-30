using UnityEngine;
using UnityEngine.AI;

public class MazeTracker : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent;
    private PlayerSafetyState safetyState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent가 없습니다.");
            enabled = false;
        }

        if (target != null)
            safetyState = target.GetComponent<PlayerSafetyState>();
    }

    void Update()
    {
        if (target == null || agent == null) return;

        // 플레이어가 세이프존에 있으면 추적 금지
        if (safetyState != null && safetyState.isInSafetyZone)
        {
            agent.SetDestination(transform.position);  // 제자리 대기
            return;
        }

        // 정상 추적
        agent.SetDestination(target.position);
    }
}
