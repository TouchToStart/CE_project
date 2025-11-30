using UnityEngine;
using System.Collections;

public class MonsterActionSound : MonoBehaviour
{
    [Header("울부짖기 AudioSource")]
    public AudioSource roarAudioSource;
    [Header("잡기 AudioSource")]
    public AudioSource catchAudioSource;

    public float roarInterval = 3.0f;
    public float maxRoarDistance = 20f;
    public float maxRoarVolume = 1.0f;

    private Coroutine roarCoroutine;
    private float minRoarDistanceSqr;

    // 이 스크립트가 MazeTracker의 target을 참조하기 위해 필요합니다.
    private MazeTracker mazeTracker;

    void Start()
    {
        // MazeTracker 참조를 가져옵니다.
        mazeTracker = GetComponent<MazeTracker>();

        // 거리 계산을 위해 제곱값 미리 계산
        minRoarDistanceSqr = maxRoarDistance * maxRoarDistance;

        StartRoaring();
    }

    // ? 새로 추가: Normal 상태로 돌아왔을 때 추적 소리를 재개합니다.
    public void ResumeChaseSounds()
    {
        // 1. 울부짖기 코루틴 재개
        StartRoaring();

        // 2. 걷기 소리 재개
        MonsterSoundControl walkController = GetComponent<MonsterSoundControl>();
        if (walkController != null && walkController.walkAudioSource != null)
        {
            if (!walkController.walkAudioSource.isPlaying)
            {
                walkController.walkAudioSource.Play();
            }
        }
    }

    public void StartRoaring()
    {
        if (roarCoroutine == null)
        {
            roarCoroutine = StartCoroutine(RoarSequence());
        }
    }

    public void StopRoaring()
    {
        if (roarCoroutine != null)
        {
            StopCoroutine(roarCoroutine);
            roarCoroutine = null;
            if (roarAudioSource != null)
            {
                roarAudioSource.Stop();
            }
        }
    }

    IEnumerator RoarSequence()
    {
        while (true)
        {
            if (roarAudioSource != null && roarAudioSource.clip != null)
            {
                roarAudioSource.PlayOneShot(roarAudioSource.clip);
            }
            yield return new WaitForSeconds(roarInterval);
        }
    }

    void Update()
    {
        // MazeTracker가 없거나 target이 없으면 볼륨 조절을 건너뜁니다.
        if (roarAudioSource == null || mazeTracker == null || mazeTracker.target == null) return;

        Transform playerTarget = mazeTracker.target;

        float distanceSqr = (playerTarget.position - transform.position).sqrMagnitude;

        if (distanceSqr > minRoarDistanceSqr)
        {
            roarAudioSource.volume = 0f;
        }
        else
        {
            float distance = Mathf.Sqrt(distanceSqr);
            float volumeRatio = 1f - (distance / maxRoarDistance);
            roarAudioSource.volume = volumeRatio * maxRoarVolume;
        }
    }

    public void PlayCatchSound()
    {
        // 잡는 순간 모든 추적 소리 중단
        StopRoaring();

        MonsterSoundControl walkController = GetComponent<MonsterSoundControl>();
        if (walkController != null && walkController.walkAudioSource != null)
        {
            walkController.walkAudioSource.Stop();
        }

        if (catchAudioSource != null)
        {
            catchAudioSource.Play();
        }
    }
}