using UnityEngine;

public class TrashCanDetector : MonoBehaviour
{
    public string trashTag = "trash";

    public GameObject removalEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(trashTag))
        {
            // 중복 처리 방지를 위해 별도의 컴포넌트나 이름을 체크
            TrashStatus status = other.GetComponent<TrashStatus>();

            if (status != null)
            {
                if (status.isCollected) return; // 이미 수거되었다면 무시
                status.isCollected = true;     // 수거됨 표시
            }
            else
            {
                // 스크립트 만들기 귀찮다면 태그를 바꿔서 재진입 방지
                other.tag = "Untagged"; 
                Destroy(other.gameObject, 5.0f);
            }
            ExcavatorGameManager.Instance.OnTrashCollected();

        }
    }
    private void OnTriggerExit(Collider other)
    {
        // 쓰레기통 안에서 밖으로 나가는 물체가 쓰레기일 때
        if (other.CompareTag(trashTag))
        {
            // 1. 파티클 효과 생성
            if (removalEffect != null)
            {
                Instantiate(removalEffect, other.transform.position, Quaternion.identity);
            }
            
            // 3. 쓰레기 오브젝트 제거
            Destroy(other.gameObject);
        }
    }
}