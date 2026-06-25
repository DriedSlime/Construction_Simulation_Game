using UnityEngine;

public class AimLine : MonoBehaviour
{
    private LineRenderer line;
    public Transform aimPoint;
    float maxDistance = 105f;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
    }

    void Update()
    {
        UpdateAimLine();
    }
    void UpdateAimLine()
    {
        Vector3 startPos = transform.position;
        line.SetPosition(0, startPos);

        RaycastHit hit;
        if (Physics.Raycast(startPos, Vector3.down, out hit, maxDistance))
        {
            // 1. 선의 끝점을 충돌 지점으로 설정
            line.SetPosition(1, hit.point);

            // 2. 조준점 활성화 및 위치 이동
            aimPoint.gameObject.SetActive(true);
            aimPoint.position = hit.point + (hit.normal * 0.1f);
        }
        else
        {
            // 닿는 곳이 없으면 선은 최대 길이로, 조준점은 숨김
            line.SetPosition(1, startPos + Vector3.down * maxDistance);
            aimPoint.gameObject.SetActive(false);
        }
    }
}