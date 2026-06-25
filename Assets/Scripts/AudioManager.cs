using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;         // 배경음
    public AudioSource engineSource;      // 굴삭기엔진
    public AudioSource craneRotateSource;   // 크레인 회전 소리
    public AudioSource craneStopSource;
    public AudioSource craneTrolleySource;

    [Header("Audio Clips")]
    AudioClip bgmClip;
    AudioClip engineIdleClip;
    public AudioClip breakClip;
    public AudioClip craneStopClip;

    float idlePitch = 1.0f;    // 정지 상태일 때 기본 피치
    float maxPitch = 1.2f;    // 전진 상태일 때 피치
    float pitchSpeed = 0.1f;   // 피치가 변화하는 속도

    private float targetPitch;
    private Coroutine fadeOutCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 게임 시작하자마자 배경음 재생
        if (bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.Play();
        }

        // 기본 공회전 소리
        if (engineIdleClip != null)
        {
            engineSource.clip = engineIdleClip;
            engineSource.Play();
        }

        targetPitch = idlePitch;
    }

    void Update()
    {
        // 매 프레임마다 현재 피치를 targetPitch를 향해 부드럽게 변경
        if (engineSource.isPlaying)
        {
            engineSource.pitch = Mathf.MoveTowards(engineSource.pitch, targetPitch, pitchSpeed * Time.deltaTime);
        }
    }

    public void SetEnginePitch(float inputMove)
    {
        // 입력값이 있으면(전진 중이면) maxPitch로, 없으면 idlePitch로 목표를 설정
        if (inputMove > 0.05f)
        {
            targetPitch = maxPitch;
        }
        else
        {
            targetPitch = idlePitch;
        }
    }

    public void PlayBreakSound(Vector3 position)
    {
        if (breakClip != null)
        {
            // 오브젝트가 파괴되어도 소리가 끊기지 않게 해당 위치에서 일회성 재생
            AudioSource.PlayClipAtPoint(breakClip, position, 10f);
        }
    }

    public void SetCraneRotationSound(bool isRotating)
    {
        if (isRotating)
        {
            if (!craneRotateSource.isPlaying) craneRotateSource.Play();
            if (fadeOutCoroutine != null)
            {
                StopCoroutine(fadeOutCoroutine);
                fadeOutCoroutine = null;
            }
        }
        else
        {
            if (craneRotateSource.isPlaying && fadeOutCoroutine == null)
            {
                fadeOutCoroutine = StartCoroutine(FadeOutAndStop(0.1f)); // 1.0f = 1초 동안 페이드아웃
            }
        }
    }

    public void SetCraneTrolleySound(bool isTrolleyMoving)
    {
        if (isTrolleyMoving)
        {
            if (!craneTrolleySource.isPlaying) craneTrolleySource.Play();
        }
        else
        {
            craneTrolleySource.Stop();
        }
    }

    private System.Collections.IEnumerator FadeOutAndStop(float duration)
    {
        float startVolume = craneRotateSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // Lerp를 이용해 현재 시간에 맞춰 볼륨을 서서히 줄이기
            craneRotateSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null; // 다음 프레임까지 대기
        }

        // 볼륨이 완전히 0이 되면 확실하게 오디오를 정지
        craneRotateSource.Stop();
        craneRotateSource.volume = 1.0f;      // 다음 재생을 위해 볼륨 원래대로 리셋
        fadeOutCoroutine = null;              // 코루틴 변수 비워주기
    }
}