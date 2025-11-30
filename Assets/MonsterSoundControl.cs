using UnityEngine;

public class MonsterSoundControl : MonoBehaviour
{
    [Header("걷기 소리 AudioSource")]
    public AudioSource walkAudioSource;

    public Transform playerTarget;
    public float maxHearingDistance = 5f;
    public float maxVolume = 1.5f;

    private float minDistanceSqr;
    private MonsterControl monsterControl;

    void Start()
    {
        if (walkAudioSource == null)
        {
            Debug.LogError("MonsterSoundControl: 걷기 소리 AudioSource를 연결해주세요.");
            enabled = false;
            return;
        }

        monsterControl = GetComponent<MonsterControl>();
        if (monsterControl == null)
        {
            Debug.LogError("MonsterControl 스크립트를 찾을 수 없어 발소리 제어가 불가능합니다!");
            enabled = false;
            return;
        }

        minDistanceSqr = maxHearingDistance * maxHearingDistance;

        // 게임 시작 시 발소리 재생을 시작합니다.
        if (!walkAudioSource.isPlaying)
        {
            walkAudioSource.Play();
        }
    }

    public void StopWalkSound()
    {
        if (walkAudioSource != null && walkAudioSource.isPlaying)
        {
            walkAudioSource.Stop();
        }
    }

    public void StartWalkSound()
    {
        if (walkAudioSource != null && !walkAudioSource.isPlaying)
        {
            walkAudioSource.Play();
        }
    }

    void Update()
    {
        if (playerTarget == null || monsterControl == null) return;


        if (monsterControl.currentState == MonsterControl.StunState.Stunned)
        {
            
            if (walkAudioSource.isPlaying)
            {
                walkAudioSource.Stop();
            }
            return; 
        }

        // Stunned 상태가 아닐 때 (Normal, Slowed)
        // 만약 멈춰있다면 다시 재생을 시작합니다.
        if (!walkAudioSource.isPlaying)
        {
            walkAudioSource.Play();
        }

        // --- 거리 기반 볼륨 조절 로직 실행 ---
        float distanceSqr = (playerTarget.position - transform.position).sqrMagnitude;

        if (distanceSqr > minDistanceSqr)
        {
            walkAudioSource.volume = 0f;
            if (walkAudioSource.isPlaying)
            {
                walkAudioSource.Stop();
            }
        }
        else
        {
            if (!walkAudioSource.isPlaying)
            {
                walkAudioSource.Play();
            }


            float distance = Mathf.Sqrt(distanceSqr);
            float volumeRatio = 1f - (distance / maxHearingDistance);
            walkAudioSource.volume = volumeRatio * maxVolume;
        }
    }
}