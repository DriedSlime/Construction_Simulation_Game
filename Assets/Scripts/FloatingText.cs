using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    float moveSpeed = 3f;       // 위로 올라가는 속도
    float fadeSpeed = 0.2f;       // 투명해지는 속도
    float duration = 5f;      // 텍스트가 유지되는 총 시간

    private TextMeshPro textMesh;
    private Color textColor;
    private Transform cameraTransform;

    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null)
        {
            textColor = textMesh.color;
        }
        if (Camera.main != null)
        {
            float cameraYRotation = Camera.main.transform.eulerAngles.y;
            
            // 카메라 화면과 평행하게 정면을 바라보도록 설정
            transform.rotation = Quaternion.Euler(0f, cameraYRotation, 0f);
        }

        Destroy(gameObject, duration);
    }

    void Update()
    {
        // 위로 천천히 이동
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.World);

        // 서서히 투명해지기
        if (textMesh != null)
        {
            textColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = textColor;
        }
    }
}