using UnityEngine;
using System.Collections;

public class HousePiece : MonoBehaviour
{
    public GameObject trashPrefab;
    public GameObject puffEffect;
    public GameObject destroyEffect;
    [Range(0f, 1f)] public float spawnChance = 0.1f;
    public float destructionDelay = 5f;

    private bool isProcessed = false; // "파괴 시작됨" 플래그

    void Update()
    {
        // Y축 추락 체크
        if (!isProcessed && transform.position.y < 0.2f)
        {
            ExecuteDestruction();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 부딪힌 오브젝트의 레이어가 "Excavator" 인지 확인
        if (collision.gameObject.CompareTag("Excavator"))
        {
            
            // 사운드 매니저를 통해 충돌 지점에서 파괴음 딱 한 번 재생
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBreakSound(transform.position);
            }

            Explode();
        }
    }

    public void Explode()
    {
        if (isProcessed) return;
        
        StartCoroutine(ReplaceWithTrashRoutine());
    }

    private IEnumerator ReplaceWithTrashRoutine()
    {
        isProcessed = true; // 여기서 true를 만들면 Update문은 더이상 실행 안 됨

        if (puffEffect != null)
        {
            Instantiate(puffEffect, transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(destructionDelay);

        FinalDestruction();
    }

    // Update(추락)에서 즉시 파괴할 때 사용
    void ExecuteDestruction()
    {
        if (isProcessed) return;
        FinalDestruction();
    }

    // 2초 뒤 혹은 추락 시 실제로 쓰레기를 만들고 사라지는 로직
    void FinalDestruction()
    {
        isProcessed = true; 
        bool hasSpawned = false;

        // 쓰레기 생성 시도
        if (trashPrefab != null)
        {
            if (Random.value <= spawnChance)
            {
                GameObject trash = Instantiate(trashPrefab, transform.position, transform.rotation);
                trash.tag = "trash"; 
                hasSpawned = true;
            }
        }

        // GameManager 알림
        if (ExcavatorGameManager.Instance != null)
        {
            ExcavatorGameManager.Instance.OnHouseDestroyed(hasSpawned);
        }

        Destroy(gameObject);
        Instantiate(destroyEffect, transform.position, Quaternion.identity);
    }


}