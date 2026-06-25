using UnityEngine;

public class TrashStatus : MonoBehaviour
{
    public bool isCollected = false; // 수거 여부 저장

        void Update()
    {
        // Y축 추락 체크 (아직 시작 안 했을 때만)
        if (transform.position.y < 0f)
        {
            Destroy(transform.gameObject);
            ExcavatorGameManager.Instance.remainingTrashes--;
        }
    }
}