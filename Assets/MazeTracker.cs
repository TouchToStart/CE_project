// using UnityEngine;
// using UnityEngine.AI;

// public class MazeTracker : MonoBehaviour
// {
//   public Transform target;
//    private NavMeshAgent agent;

//     void Start()
//     {
//          agent = GetComponent<NavMeshAgent>();
//          if (agent == null)
//        {
//             Debug.LogError("NavMeshAgent 컴포넌트가 Game Object에 없습니다.");
//              enabled = false; // 스크립트 비활성화
//         }
//     }

//     // 여기에 각 패트롤 지점을 정해놓고 랜덤으로 목표를 정해서 가다가 플레이어가 소리를 내거나 만났을 경우 enum을 변경해서 추적하는 알고리즘으로 작성하면 좋을거 같다. 
//    void Update()
//      {
//          if (target != null && agent != null)
//          {
//              agent.SetDestination(target.position);
//         }

//      }
//  }



// 2025.11.16

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
