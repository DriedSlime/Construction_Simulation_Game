using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CraneTargetZone : MonoBehaviour
{
    public int scorePerBuilding = 100;
    public float requiredTime = 3f;     // 필요한 대기 시간

    public GameObject redVisual;    // 빨간색 테두리/구역 오브젝트
    public ParticleSystem successParticle;     // 성공 이펙트
    
    // 현재 구역 안에 들어와 있는 건물들과 그들의 대기 시간을 추적
    private Dictionary<GameObject, float> buildingTimers = new Dictionary<GameObject, float>();
    private BoxCollider zoneCollider;

    void Start()
    {
        zoneCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Building") && other.transform.parent == null)
        {
            // 타겟 존이 건물 안에 완전히 포함
            if (IsFullyContained(other))
            {
                if (!buildingTimers.ContainsKey(other.gameObject))
                {
                    buildingTimers.Add(other.gameObject, 0f);
                }

                buildingTimers[other.gameObject] += Time.deltaTime;

                if (buildingTimers[other.gameObject] >= requiredTime)
                {
                    Success(other.gameObject);
                }
            }
            else
            {
                if (buildingTimers.ContainsKey(other.gameObject))
                {
                    buildingTimers.Remove(other.gameObject);

                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (buildingTimers.ContainsKey(other.gameObject))
        {
            buildingTimers.Remove(other.gameObject);
        }
    }

    // 건물이 박스 콜라이더 안에 완전히 들어왔는지 판정하는 함수
    private bool IsFullyContained(Collider buildingCol)
    {
        if (zoneCollider == null) zoneCollider = GetComponent<BoxCollider>();

        // 작은 타겟 존의 중심점, 최소점(min), 최대점(max) 경계
        Bounds zoneBounds = zoneCollider.bounds;
        Vector3 zoneCenter = zoneBounds.center;
        Vector3 zoneMin = zoneBounds.min;
        Vector3 zoneMax = zoneBounds.max;

        bool centerInside = buildingCol.ClosestPoint(zoneCenter) == zoneCenter;
        bool minInside    = buildingCol.ClosestPoint(zoneMin)    == zoneMin;
        bool maxInside    = buildingCol.ClosestPoint(zoneMax)    == zoneMax;

        // 중심점과 경계선들이 전부 건물 안에 완전히 파묻혀 있을 때만 true를 반환
        return centerInside && minInside && maxInside;
    }

    private void Success(GameObject building)
    {
        CraneScoreManager.instance.AddScore(scorePerBuilding);
        
        if (building.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }
        building.tag = "Untagged";
        buildingTimers.Remove(building);

        
        if (redVisual != null)
        {
            redVisual.SetActive(false);
        }

        // 이펙트
        if (successParticle != null)
        {
            Quaternion spawnRotation = Quaternion.Euler(-90, 0, 0);
            ParticleSystem effectInstance = Instantiate(successParticle, zoneCollider.bounds.center, spawnRotation);
            effectInstance.Play();
            
            Destroy(effectInstance.gameObject, effectInstance.main.duration);
        }
    }
}
