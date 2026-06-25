using UnityEngine;

public class Trees : MonoBehaviour
{
    bool isHit = false;

    [Header("UI Settings")]
    public GameObject floatingTextPrefab;
    Vector3 textOffset = new Vector3(0, 8f, 0); // 나무에서 텍스트가 뜰 위치

    public AudioClip crackSound;
    AudioSource audioSource;

    Rigidbody rb;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Excavator") && !isHit)
        {
            rb.isKinematic = false;
            isHit = true;

            // 타이머 증가
            if (ExcavatorGameManager.Instance != null)
            {
                ExcavatorGameManager.Instance.timer += 30;
            }

            SpawnFloatingText();
            PlayCrackSound();
        }       
    }

    //둥둥 떠다니는 텍스트 생성
    private void SpawnFloatingText()
    {
        if (floatingTextPrefab != null)
        {
            // 나무 위치보다 조금 위
            Vector3 spawnPosition = transform.position + textOffset;

            GameObject textObj = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private void PlayCrackSound()
    {
        if (audioSource != null && crackSound != null)
        {
            audioSource.PlayOneShot(crackSound);
        }
    }
}