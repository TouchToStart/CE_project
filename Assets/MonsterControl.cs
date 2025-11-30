using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MonsterControl : MonoBehaviour
{
    public enum StunState { Normal, Slowed, Stunned }
    public StunState currentState = StunState.Normal;

    private NavMeshAgent navMeshAgent;
    private MazeTracker mazeTracker;
    private Animator animator;
    private float originalSpeed;
    private Coroutine stunCoroutine;
    private MonsterActionSound actionSound;
    private MonsterSoundControl soundControl;

    public float initialSlowDuration = 1.0f; 
    public float slowedSpeed = 2.0f;        
    public float fullStunDuration = 5.0f;   



    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        mazeTracker = GetComponent<MazeTracker>();
        animator = GetComponentInChildren<Animator>();
        actionSound = GetComponent<MonsterActionSound>();
        soundControl = GetComponent<MonsterSoundControl>(); 

        if (navMeshAgent != null)
        {
            originalSpeed = navMeshAgent.speed;
        }
        SetMonsterState(StunState.Normal);
    }

    public bool IsFullyStunned()
    {
        return currentState == StunState.Stunned;
    }

    public void StunStart()
    {
        // 이미 둔화/무력화 시퀀스가 진행 중이면 중복 시작을 막고 시퀀스를 유지합니다.
        if (stunCoroutine != null)
        {
            return;
        }
        stunCoroutine = StartCoroutine(HandleStunSequence());
    }

    public void StunEnd()
    {
        // Stunned 상태일 경우: 강제 유지 (무시)
        if (currentState == StunState.Stunned)
        {
            return;
        }

        // Slowed 상태일 경우: 시퀀스 중단 후 Normal 복귀
        if (currentState == StunState.Slowed)
        {
            if (stunCoroutine != null)
            {
                StopCoroutine(stunCoroutine);
                stunCoroutine = null;
            }
            SetMonsterState(StunState.Normal);
        }
    }

    private void SetMonsterState(StunState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        Renderer renderer = GetComponentInChildren<Renderer>();

        switch (currentState)
        {
            case StunState.Normal:
                if (navMeshAgent != null) { navMeshAgent.isStopped = false; navMeshAgent.speed = originalSpeed; }
                if (mazeTracker != null) mazeTracker.enabled = true;
                if (animator != null) animator.speed = 1f;
                if (renderer != null) renderer.material.color = Color.white;

                if (actionSound != null) actionSound.ResumeChaseSounds();
                if (soundControl != null) soundControl.StartWalkSound(); 

                break;
            case StunState.Slowed:
                // 이동 속도를 2.0으로 설정
                if (navMeshAgent != null) { navMeshAgent.isStopped = false; navMeshAgent.speed = slowedSpeed; }
                if (mazeTracker != null) mazeTracker.enabled = true;
                if (animator != null) animator.speed = 1f;
                if (renderer != null) renderer.material.color = Color.yellow;
                if(soundControl != null) soundControl.StartWalkSound();
                break;
            case StunState.Stunned:
                // 이동/추적/애니메이션 정지
                if (navMeshAgent != null) navMeshAgent.isStopped = true;
                if (mazeTracker != null) mazeTracker.enabled = false;
                if (animator != null) animator.speed = 0f;
                if (renderer != null) renderer.material.color = Color.blue;
                if(soundControl == null) soundControl.StopWalkSound();
                break;
        }
    }

    private IEnumerator HandleStunSequence()
    {
        // 1단계: 둔화 (1.0초)
        SetMonsterState(StunState.Slowed);
        yield return new WaitForSeconds(initialSlowDuration);

        // 2단계: 완전 무력화 (5.0초)
        SetMonsterState(StunState.Stunned);
        yield return new WaitForSeconds(fullStunDuration);

        // 3단계: Normal 상태로 복귀
        SetMonsterState(StunState.Normal);
        stunCoroutine = null;
    }
}