using UnityEngine;

public class BucketCollisionHandler : MonoBehaviour
{ 
    public GameObject trashPrefab;
    float impactForce = 10f;          // 기둥을 밀어내는 힘

    string propsTag = "Props";        // 소품 태그 
    string wallTag = "Wall";        // 벽 태그
    string pillarTag = "Pillar";      // 기둥 태그

    private void OnCollisionEnter(Collision collision)
    {
        // 1. Pillar 레이어와 충돌 시: 물리 엔진 활성화
        if (collision.gameObject.CompareTag(wallTag)  || collision.gameObject.CompareTag(pillarTag) || collision.gameObject.CompareTag(propsTag))
        {
            Rigidbody pillarRb = collision.gameObject.GetComponent<Rigidbody>();
            if (pillarRb != null && pillarRb.isKinematic)
            {
                pillarRb.isKinematic = false;
                
                // 부딪힌 방향으로 약간의 힘을 가해 자연스럽게 넘어뜨림
                Vector3 pushDir = collision.relativeVelocity;
                pillarRb.AddForce(pushDir * impactForce, ForceMode.Impulse);
                
            }
            
            HousePiece piece = collision.gameObject.GetComponent<HousePiece>();
            if (piece != null)
            {
                piece.Explode();
                return;
            }
            else
            {
                // 스크립트가 없다면 그냥 즉시 삭제
                Destroy(collision.gameObject);
            }
        }
    }
}